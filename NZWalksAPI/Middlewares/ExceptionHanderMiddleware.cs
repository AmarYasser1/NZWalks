using System.Net;

namespace NZWalks.API.Middlewares
{
    public class ExceptionHanderMiddleware
    {
        private readonly ILogger<ExceptionHanderMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHanderMiddleware(ILogger<ExceptionHanderMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }catch(Exception ex)
            {
                var errorId = Guid.NewGuid();
                _logger.LogError(ex, $"{errorId} : {ex.Message}");

                httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var customError = new
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong! We are looking into resolving it."
                };

                await httpContext.Response.WriteAsJsonAsync(customError);
            }
        }
    }
}
