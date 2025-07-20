using NZWalks.API.Models.DTOs;

namespace NZWalks.API.Services
{
    public interface IRegionService
    {
        Task<IEnumerable<RegionDto>> GetAllAsync(CancellationToken cancellationToken = default, bool tracked = true);
        Task<RegionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracked = true);
        Task<RegionDto?> CreateAsync(AddRegionRequestDto regionRequestDto, CancellationToken cancellationToken = default);
        Task<RegionDto?> UpdateAsync(Guid id, UpdateRegionRequestDto updateRegionRequestDto, CancellationToken cancellationToken = default);
        Task<RegionDto?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
