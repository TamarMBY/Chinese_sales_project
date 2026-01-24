using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Services;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PackageResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PackageResponseDto>>> GetAll()
        {
            var packages = await _packageService.GetAll();
            return Ok(packages);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PackageResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {
            var package = await _packageService.GetById(id);
            if (package == null)
            {
                return NotFound(new { message = $"package with ID {id} not exist" });
            }
            return Ok(package);
        }
        [HttpPost]
        [ProducesResponseType(typeof(PackageResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddPackage([FromBody] PackageCreateDto createDto)
        {
            try
            {
                var package = await _packageService.AddPackage(createDto);
                return Ok(package);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(PackageResponseDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var reasult = await _packageService.DeletePackage(id);
            if (!reasult)
            {
                return NotFound(new { message = $"package with Id {id} not found" });
            }
            return NoContent();
        }

        [HttpPut("{packageId}")]
        [ProducesResponseType(typeof(PackageResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePackage([FromRoute] int packageId, [FromBody] PackageUpdateDto updateDto)
        {
            try
            {
                var package = await _packageService.UpdatePackage(packageId, updateDto);
                return Ok(package);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpGet("sortPackages")]
        [ProducesResponseType(typeof(PackageResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PackageResponseDto>>> SortPackages([FromQuery] string? sortBy)
        {
            var gifts = await _packageService.SortPackages(sortBy);
            return Ok(gifts);
        }
    }
}
