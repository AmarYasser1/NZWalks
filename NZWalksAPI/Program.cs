
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using NZWalks.API.Configurations;
using NZWalks.API.Extensions;
using NZWalks.API.Mappings;
using NZWalks.API.Middlewares;
using NZWalks.API.Repositories.V1;
using NZWalks.API.Repositories.V2;
using NZWalks.API.Services;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NZWalksAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.AddLoggingSerilorConfiguration();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddNZWalksDbContext(builder.Configuration);
            builder.Services.AddNZWalksAuthDbContext(builder.Configuration);

            builder.Services.AddScoped<NZWalks.API.Repositories.V1.IRegionRepository, NZWalks.API.Repositories.V1.SQLRegionRepository>();
            builder.Services.AddScoped<NZWalks.API.Repositories.V1.IWalkRepository, NZWalks.API.Repositories.V1.SQLWalkRepository>();
            builder.Services.AddScoped<ITokenRepository, TokenRepository>();
            builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

            builder.Services.AddRedisConfiguration(builder.Configuration);

            builder.Services.AddInjectionV2();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddControllers();

            builder.Services.AddApiVersioningConfiguration();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGenConfiguration();


            builder.Services.AddHealthChecks()
                            .AddSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString"))
                            .AddRedis(builder.Configuration.GetConnectionString("Redis"));

            builder.Services.AddIdentityConfiguration();
            builder.Services.AddAuthenticationJwtToken(builder.Configuration);
                

            builder.Services.ConfigureOptions<SwaggerOptionsConfiguration>();
            var app = builder.Build();

            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            $"My API {description.GroupName.ToUpperInvariant()}");
                    }
                });
            }

            app.UseMiddleware<ExceptionHanderMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
                RequestPath = "/Images"
            });
            app.MapControllers();

            app.Run();
        }
    }
}

// http://localhost:8080/swagger
