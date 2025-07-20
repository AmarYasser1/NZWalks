using AutoMapper;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTOs;
using NZWalks.API.Repositories.V2;

namespace NZWalks.API.Services
{
    public class WalkService : IWalkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WalkService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<WalkDto?> CreateAsync(AddWalkRequestDto addWalkRequestDto, CancellationToken cancellationToken = default)
        {
            var walkModel = _mapper.Map<Walk>(addWalkRequestDto);
            walkModel.Id = Guid.NewGuid();

            await _unitOfWork.WalkRepository.AddAsync(walkModel, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return _mapper.Map<WalkDto>(walkModel);
        }

        public async Task<WalkDto?> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
           var deletedWalk = await _unitOfWork.WalkRepository.GetByIdAsync(id);

            if (deletedWalk is null)
                return null;

            _unitOfWork.WalkRepository.Remove(deletedWalk);
            await _unitOfWork.SaveAsync(cancellationToken);

            return _mapper.Map<WalkDto>(deletedWalk);
        }

        public async Task<IEnumerable<WalkDto>> GetAllAsync(WalkQueryParameters walkQueryParameters, 
            CancellationToken cancellationToken = default, bool tracked = true)
        {
            // This is include prop by default , then I will optimize it for future
            var walksModel = await _unitOfWork.WalkRepository.GetSpecialAllAsync(walkQueryParameters, cancellationToken,tracked: tracked);

            return _mapper.Map<List<WalkDto>>(walksModel);
        }

        public async Task<WalkDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracked = true)
        {
            var walkModel = await _unitOfWork.WalkRepository.GetByIdAsync(id, 
                cancellationToken: cancellationToken, includeProperities: "Difficulty,Region", tracked: tracked);

            if (walkModel is null)
                return null;

            return _mapper.Map<WalkDto>(walkModel);
        }

        public async Task<WalkDto?> UpdateAsync(Guid id, UpdateWalkRequestDto updateWalkRequestDto, CancellationToken cancellationToken = default)
        {
            var walkModel = _mapper.Map<Walk>(updateWalkRequestDto);

            var updatedWalkModel = await _unitOfWork.WalkRepository.UpdateAsync(id, walkModel, cancellationToken);
           
            if(updatedWalkModel is null) 
                return null;

            await _unitOfWork.SaveAsync(cancellationToken);
            return _mapper.Map<WalkDto>(updatedWalkModel);
        }
    }
}
