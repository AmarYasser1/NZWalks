using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTOs;

namespace NZWalks.API.Services
{
    public interface IWalkService
    {
        Task<IEnumerable<WalkDto>> GetAllAsync(WalkQueryParameters walkQueryParameters, 
            CancellationToken cancellationToken = default, bool tracked = true);
        Task<WalkDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracked = true);
        Task<WalkDto?> CreateAsync(AddWalkRequestDto addWalkRequestDto, CancellationToken cancellationToken = default);
        Task<WalkDto?> UpdateAsync(Guid id, UpdateWalkRequestDto updateWalkRequestDto, CancellationToken cancellationToken = default);
        Task<WalkDto?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
