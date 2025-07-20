using AutoMapper;
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
    public class WalksController : ControllerBase
    {
        private readonly IWalkRepository _walkRepository;
        private readonly IMapper _mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            _walkRepository = walkRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] WalkQueryParameters walkQueryParameters)
        {
            var walksModel = await _walkRepository.GetAllAsync(walkQueryParameters);

            return Ok(_mapper.Map<List<WalkDto>>(walksModel));
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkModel = await _walkRepository.GetByIdAsync(id);

            if (walkModel is null)
                return NotFound();

            return Ok(_mapper.Map<WalkDto>(walkModel));
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            var walkModel = _mapper.Map<Walk>(addWalkRequestDto);
            
            var addedWalkModel = await _walkRepository.CreateAsync(walkModel);

            var walkDto = _mapper.Map<WalkDto>(addedWalkModel);

            return CreatedAtAction(nameof(GetById), new {id = walkDto.Id}, walkDto);
        }

        [HttpPut("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            var updatedWalkModel = _mapper.Map<Walk>(updateWalkRequestDto);

            updatedWalkModel = await _walkRepository.UpdateAsync(id, updatedWalkModel);

            if (updatedWalkModel is null) 
                return NotFound();

            return Ok(_mapper.Map<WalkDto>(updatedWalkModel));
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedModel = await _walkRepository.DeleteAsync(id);

            if (deletedModel is null)
                return NotFound();

            return Ok(_mapper.Map<WalkDto>(deletedModel));
        }
    }
}
