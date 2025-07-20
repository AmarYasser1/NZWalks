using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTOs;
using NZWalks.API.Repositories.V1;

namespace NZWalks.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [HttpPost("Upload")]
        [ValidateModel]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto imageUploadRequestDto)
        {
            ValidateFileUpload(imageUploadRequestDto);

            if (ModelState.IsValid)
            {
                var imageDomain = new Image 
                {
                    File = imageUploadRequestDto.File,
                    FileName = imageUploadRequestDto.FileName,
                    FileExtension = Path.GetExtension(imageUploadRequestDto.File.FileName),
                    FileDescription = imageUploadRequestDto.FileDescription,
                    FileSizeInBytes = imageUploadRequestDto.File.Length
                };

                imageDomain = await _imageRepository.Upload(imageDomain);
                return Ok(imageDomain);
            }

            return BadRequest(ModelState);
        }

        private  void ValidateFileUpload(ImageUploadRequestDto imageUploadRequestDto)
        {
            var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
            if (!allowedExtensions.Contains(Path.GetExtension(imageUploadRequestDto.File.FileName).ToLower()))
                ModelState.AddModelError("file", "Unsupported file extension.");

            if (imageUploadRequestDto.File.Length > 10485760)
                ModelState.AddModelError("file", "File size is bigger than 10MB , please upload smaller size.");
        }
    }
}
