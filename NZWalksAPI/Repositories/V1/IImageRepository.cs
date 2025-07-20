using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories.V1
{
    public interface IImageRepository
    {
        Task<Image> Upload(Image image);
    }
}
