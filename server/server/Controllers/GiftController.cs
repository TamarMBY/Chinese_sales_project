using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Repositories;
using server.Services;
using System.Text;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;
        private readonly IDistributedCache _cache;

        public GiftController(IGiftService giftService, IDistributedCache cache)
        {
            _giftService = giftService;
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GiftResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GiftResponseDto>>> GetAll()
        {
            var cacheKey = "all-gifts";

            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("CACHE HIT");
                var giftsFromCache = JsonSerializer.Deserialize<IEnumerable<GiftResponseDto>>(cachedData);
                return Ok(giftsFromCache);
            }

            _logger.LogInformation("CACHE MISS");

            var gifts = await _giftService.GetAll();

            var serializedData = JsonSerializer.Serialize(gifts);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300)
            };
            await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

            return Ok(gifts);
        }
        [HttpPost]
        //[Authorize(Roles = "Admin")]
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
        //[Authorize(Roles = "Admin")]
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
        //[Authorize(Roles = "Admin")]
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

        [HttpPut("lottery")]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GiftRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Lottery([FromBody] int giftId)
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

        [HttpGet("download-report")]
        public async Task<IActionResult> DownloadReport()
        {
            try
            {
                byte[] reportData = await _giftService.GenerateWinnersReport();
                string fileName = $"WinnersReport_{DateTime.Now:yyyyMMdd}.csv";

                return File(reportData, "text/csv; charset=utf-8", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "אירעה שגיאה ביצירת הדוח: " + ex.Message);
            }
        }

    }
}
