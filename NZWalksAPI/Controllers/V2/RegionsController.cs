using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.DTOs;
using NZWalks.API.Services;

namespace NZWalks.API.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    [AllowAnonymous]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;
        private readonly ILogger<RegionsController> _logger;
        private readonly ICacheService _cacheService;

        public RegionsController(ILogger<RegionsController> logger, IRegionService regionService, ICacheService cacheService)
        {
            _regionService = regionService;
            _logger = logger;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var cachedRegions = await _cacheService.GetDataAsync<IEnumerable<RegionDto>>("Regions", cancellationToken);

            if (cachedRegions is not null)
            {
                _logger.LogInformation("Cache hit: Regions");
                return Ok(cachedRegions);
            }

            _logger.LogInformation("Cache miss: fetching Regions from DB");

            var regionsDto = await _regionService.GetAllAsync(cancellationToken, tracked: false);
            
            await _cacheService.SetDataAsync("Regions", regionsDto, cancellationToken: cancellationToken);

            return Ok(regionsDto);
        }

        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var cachedKey = $"region-{id}";
            var cachedRegion = await _cacheService.GetDataAsync<RegionDto>(cachedKey, cancellationToken);

            if(cachedRegion is not null)
                return Ok(cachedRegion);


            var regionDto = await _regionService.GetByIdAsync(id, cancellationToken, tracked: false);
            if (regionDto is null)
                    return NotFound();

            await _cacheService.SetDataAsync(cachedKey, regionDto, cancellationToken:cancellationToken);
            return Ok(regionDto);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto, CancellationToken cancellationToken)
        {
            var newRegionDto = await _regionService.CreateAsync(addRegionRequestDto, cancellationToken);

            if (newRegionDto is null)
                return BadRequest();

            await _cacheService.RemoveDataAsync("Regions");

            _logger.LogInformation("New Region Created Successfully!");
             return CreatedAtAction(nameof(GetById), new { id = newRegionDto.Id }, newRegionDto);
        }

        [HttpPut("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto, CancellationToken cancellationToken)
        {
            var updatedRegionDto = await _regionService.UpdateAsync(id, updateRegionRequestDto, cancellationToken);

            if (updatedRegionDto is null)
                return NotFound();

            await _cacheService.RemoveDataAsync("Regions");
            await _cacheService.RemoveDataAsync($"region-{id}");

            _logger.LogInformation($"Update region with id {id}");
            return Ok(updatedRegionDto);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteById(Guid id,CancellationToken cancellationToken) 
        { 
            var deletedRegionDto = await _regionService.DeleteAsync(id, cancellationToken);

            if (deletedRegionDto is null)
                return NotFound();

            await _cacheService.RemoveDataAsync("Regions");
            await _cacheService.RemoveDataAsync($"region-{id}");

            _logger.LogInformation($"Delete region with id : {id}");
            return Ok(deletedRegionDto);
        }
    }
}
