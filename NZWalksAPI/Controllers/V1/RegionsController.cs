using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    //[AllowAnonymous]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RegionsController> _logger;
        public RegionsController(IRegionRepository regionRepository, IMapper mapper, ILogger<RegionsController> logger)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var regions = await _regionRepository.GetAllAsync();

                var regionsDto = _mapper.Map<List<RegionDto>>(regions);

                return Ok(regionsDto);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, ex.Message);
                throw;
            } 
        }

        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var region = await _regionRepository.GetByIdAsync(id);

            if (region is null)
                    return NotFound();

            return Ok(_mapper.Map<RegionDto>(region));
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
             var regionDomain = _mapper.Map<Region>(addRegionRequestDto);

             regionDomain = await _regionRepository.CreateAsync(regionDomain);

             var regionDtoResponse = _mapper.Map<RegionDto>(regionDomain);

             return CreatedAtAction(nameof(GetById), new { id = regionDtoResponse.Id }, regionDtoResponse);
        }

        [HttpPut("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var regionModel = _mapper.Map<Region>(updateRegionRequestDto);

            regionModel = await _regionRepository.UpdateAsync(id, regionModel);

            if (regionModel is null)
                return NotFound();

            return Ok(_mapper.Map<RegionDto>(regionModel));
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteById(Guid id) 
        { 
            var deletedRegion = await _regionRepository.DeleteAsync(id);

            if (deletedRegion is null)
                return NotFound();

            return Ok(_mapper.Map<RegionDto>(deletedRegion));
        }
    }
}
