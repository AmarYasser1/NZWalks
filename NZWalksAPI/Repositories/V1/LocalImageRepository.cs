using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories.V1
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly NZWalksDbContext _context;
        public LocalImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor contextAccessor, NZWalksDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _contextAccessor = contextAccessor;
            _context = context;
        }
        public async Task<Image> Upload(Image image)
        {
            string fileNameWithExtension = image.FileName + image.FileExtension;
            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", fileNameWithExtension);

            // Upload Image to Local Path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            //https://localhost:7321/images/image.jpg
            var urlFilePath = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}{_contextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath;

            await _context.AddAsync(image);
            await _context.SaveChangesAsync();

            return image;
        }
    }
}
