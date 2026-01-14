using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Interfaces;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorController : ControllerBase
    {
        private readonly IDonorService _donorService;

        public DonorController(IDonorService donorService)
        {
            _donorService = donorService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DonorRespnseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DonorRespnseDto>>> GetAll()
        {
            var donors = await _donorService.GetAll();
            return Ok(donors);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DonorRespnseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(string id)
        {
            var donor = await _donorService.GetById(id);
            if (donor == null)
            {
                return NotFound(new { message = $"donor with ID {id} not exist" });
            }
            return Ok(donor);
        }
        [HttpPost]
        [ProducesResponseType(typeof(DonorRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddDonor([FromBody] DonorCreateDto createDto)
        {
            try
            {
                var donor = await _donorService.AddDonor(createDto);
                return Ok(donor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(DonorRespnseDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDonor (string id)
        {
            var reasult = await _donorService.DeleteDonor(id);
            if (!reasult)
            {
                return NotFound(new { message = $"donor with Id {id} not found" });
            }
            return NoContent();
        }

        [HttpPut("{donorId}")]
        [ProducesResponseType(typeof(DonorRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateDonor([FromRoute] string donorId,[FromBody]DonorUpdateDto createDto)
        {
            try
            {
                var donor = await _donorService.UpdateDonor(donorId,createDto);
                return Ok(donor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpGet("FilterDonors")]
        [ProducesResponseType(typeof(IEnumerable<DonorRespnseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DonorRespnseDto>>> FilterDoners(
            [FromQuery] string? name, [FromQuery] string? email, [FromQuery] int? giftId
            )
        {
            var donors = await _donorService.FilterDonors(name, email, giftId);
            return Ok(donors);
        }

    }
}
