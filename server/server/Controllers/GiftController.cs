using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Interfaces;
using server.Services;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GiftRespnseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GiftRespnseDto>>> GetAll()
        {
            var gifts = await _giftService.GetAll();
            return Ok(gifts);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GiftRespnseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {
            var gift = await _giftService.GetById(id);
            if (gift == null)
            {
                return NotFound(new { message = $"Gift with ID {id} not exist" });
            }
            return Ok(gift);
        }
        [HttpPost]
        [ProducesResponseType(typeof(GiftRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddGift([FromBody] GiftCreateDto createDto)
        {
            try
            {
                var gift = await _giftService.AddGift(createDto);
                return Ok(gift);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(GiftRespnseDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGift(int id)
        {
            var reasult = await _giftService.DeleteGift(id);
            if (!reasult)
            {
                return NotFound(new { message = $"Gift with Id {id} not found" });
            }
            return NoContent();
        }
        [HttpPut("{giftId}")]
        [ProducesResponseType(typeof(GiftRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateGift([FromRoute] int giftId,[FromBody] GiftUpdateDto updateDto)
        {
            try
            {
                var gift = await _giftService.UpdateGift(giftId,updateDto);
                return Ok(gift);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }

        [HttpPut("lottery/{giftId}")]
        [ProducesResponseType(typeof(GiftRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Lottery([FromRoute] int giftId)
        {
            try
            {
                var gift = await _giftService.Lottery(giftId);
                return Ok(gift);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpGet("filterGifts")]
        [ProducesResponseType(typeof(GiftRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<GiftRespnseDto>>> FilterGifts(
           [FromQuery] string? giftName, [FromQuery] int? categoryId, [FromQuery] string? donerName, [FromQuery] int? buyersCount
           )
        {
            Console.WriteLine("aaaa");
            var gifts = await _giftService.FilterGifts(giftName, categoryId, donerName, buyersCount);
            return Ok(gifts);
        }

    }
}
