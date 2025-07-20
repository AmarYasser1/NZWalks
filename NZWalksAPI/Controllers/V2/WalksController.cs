using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTOs;
using NZWalks.API.Services;

namespace NZWalks.API.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class WalksController : ControllerBase
    {
        private readonly IWalkService _walkService;
        private readonly ILogger<WalksController> _logger;
        private readonly ICacheService _cacheService;

        public WalksController(IWalkService walkService, ILogger<WalksController> logger, ICacheService cacheService)
        {
            _walkService = walkService;
            _logger = logger;
            _cacheService = cacheService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] WalkQueryParameters walkQueryParameters, CancellationToken cancellationToken)
        {
            var cachedCaches = await _cacheService.GetDataAsync<IEnumerable<WalkDto>>("Walks", cancellationToken);

            if(cachedCaches is not null)
                return Ok(cachedCaches);

            var walksDto = await _walkService.GetAllAsync(walkQueryParameters, cancellationToken);

            await _cacheService.SetDataAsync("Walks", walksDto, cancellationToken:cancellationToken);
            return Ok(walksDto);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var cachedKey = $"walk-{id}";

            var cahcedWalk = await _cacheService.GetDataAsync<WalkDto>(cachedKey, cancellationToken);
            if(cahcedWalk is not null)
                return Ok(cahcedWalk);

            var walkDto = await _walkService.GetByIdAsync(id, cancellationToken);

            if (walkDto is null)
                return NotFound();

            await _cacheService.SetDataAsync(cachedKey, walkDto, cancellationToken:cancellationToken);
            return Ok(walkDto);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto, CancellationToken cancellationToken)
        {
            var createdWalkDto = await _walkService.CreateAsync(addWalkRequestDto, cancellationToken);

            if (createdWalkDto is null)
                return NotFound();

            await _cacheService.RemoveDataAsync("Walks", cancellationToken);

            _logger.LogInformation("New Walk created successfully!");
            return CreatedAtAction(nameof(GetById), new {id = createdWalkDto.Id}, createdWalkDto);
        }

        [HttpPut("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto, CancellationToken cancellationToken)
        {
            var updatedWalkDto = await _walkService.UpdateAsync(id, updateWalkRequestDto, cancellationToken);

            if (updatedWalkDto is null) 
                return NotFound();

            await _cacheService.RemoveDataAsync("Walks", cancellationToken);
            await _cacheService.RemoveDataAsync($"walk-{id}", cancellationToken);
            return Ok(updatedWalkDto);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var deletedWalkDto = await _walkService.DeleteAsync(id, cancellationToken);

            if (deletedWalkDto is null)
                return NotFound();

            await _cacheService.RemoveDataAsync("Walks", cancellationToken);
            await _cacheService.RemoveDataAsync($"walk-{id}", cancellationToken);
            return Ok(deletedWalkDto);
        }
    }
}
