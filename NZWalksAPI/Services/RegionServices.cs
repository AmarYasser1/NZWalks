using AutoMapper;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTOs;
using NZWalks.API.Repositories.V2;

namespace NZWalks.API.Services
{
    public class RegionServices : IRegionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RegionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<RegionDto?> CreateAsync(AddRegionRequestDto addRegionRequestDto, CancellationToken cancellationToken = default)
        {
            var regionModel = _mapper.Map<Region>(addRegionRequestDto);
            regionModel.Id = Guid.NewGuid();

            await _unitOfWork.RegionRepository.AddAsync(regionModel);
            await _unitOfWork.SaveAsync(cancellationToken);

            return _mapper.Map<RegionDto>(regionModel);
        }

        public async Task<RegionDto?> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var regionModel = await _unitOfWork.RegionRepository.GetByIdAsync(id);

            if (regionModel is null)
                return null;

            _unitOfWork.RegionRepository.Remove(regionModel);
            await _unitOfWork.SaveAsync(cancellationToken);

            return _mapper.Map<RegionDto>(regionModel);
        }

        public async Task<IEnumerable<RegionDto>> GetAllAsync(CancellationToken cancellationToken = default, bool tracked = true)
        {
            var regionsModel = await _unitOfWork.RegionRepository.GetAllAsync(tracked: tracked);

            return _mapper.Map<List<RegionDto>>(regionsModel);
        }

        public async Task<RegionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool tracked = true)
        {
            var regionModel = await _unitOfWork.RegionRepository.GetByIdAsync(id, tracked: tracked);

            if (regionModel is null)
                return null;

            return _mapper.Map<RegionDto>(regionModel);
        }

        public async Task<RegionDto?> UpdateAsync(Guid id, UpdateRegionRequestDto updateRegionRequestDto, CancellationToken cancellationToken = default)
        {
            var regionModel = _mapper.Map<Region>(updateRegionRequestDto);

            var updatedRegionModel = await _unitOfWork.RegionRepository.UpdateAsync(id, regionModel, cancellationToken: cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return _mapper.Map<RegionDto>(updatedRegionModel);
        }
    }
}
