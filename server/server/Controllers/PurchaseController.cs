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
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PurchaseRespnseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PurchaseRespnseDto>>> GetAll()
        {
            var purchases = await _purchaseService.GetAll();
            return Ok(purchases);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PurchaseRespnseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {
            var purchase = await _purchaseService.GetById(id);
            if (purchase == null)
            {
                return NotFound(new { message = $"Purchase with ID {id} not exist" });
            }
            return Ok(purchase);
        }
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(PurchaseRespnseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetByUsesrId(string userId)
        {
            var purchase = await _purchaseService.GetByUserId(userId);
            if (purchase == null)
            {
                return NotFound(new { message = $"Purchase with ID {userId} not exist" });
            }
            return Ok(purchase);
        }
        [HttpPost]
        [ProducesResponseType(typeof(PurchaseRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddPurchase([FromBody] PurchaseCreateDto createDto)
        {
            try
            {
                var purchase = await _purchaseService.AddPurchase(createDto);
                return Ok(purchase);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(PurchaseRespnseDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var reasult = await _purchaseService.DeletePurchase(id);
            if (!reasult)
            {
                return NotFound(new { message = $"Purchase with Id {id} not found" });
            }
            return NoContent();
        }
        [HttpPut("{purchaseId}")]
        [ProducesResponseType(typeof(PurchaseRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePurchase([FromRoute] int purchaseId,[FromBody] PurchaseUpdateDto updateDto)
        {
            try
            {
                var purchase = await _purchaseService.UpdatePurchase(purchaseId,updateDto);
                return Ok(purchase);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpPost("{purchaseId}/packages")]
        [ProducesResponseType(typeof(PurchaseRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddPackageToPurchase([FromRoute] int purchaseId, [FromBody] Package package)
        {
            try
            {
                var purchase = await _purchaseService.AddPackageToPurchase(purchaseId, package);
                return Ok(purchase);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpDelete("{purchaseId}/{packageId}")]
        [ProducesResponseType(typeof(PurchaseRespnseDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePackageFromPurchase([FromRoute] int purchaseId, [FromRoute] int packageId)
        {
            var reasult = await _purchaseService.DeletePackageFromPurchase(purchaseId, packageId);
            if (reasult == null)
            {
                return NotFound(new { message = $"Package with Id {packageId} not found" });
            }
            return NoContent();
        }
        [HttpPost("addTicket")]
        [ProducesResponseType(typeof(PurchaseRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddTicketToPurchase([FromBody] TicketCreateDto tCreateDto)
        {
            try
            {
                var purchase = await _purchaseService.AddTicketToPurchase(tCreateDto);
                return Ok(purchase);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpDelete("{purchaseId}/{ticketId}")]
        [ProducesResponseType(typeof(PurchaseRespnseDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTicket([FromRoute] int purchaseId,[FromRoute] int ticketId)
        {
            var reasult = await _purchaseService.DeleteTicket(purchaseId, ticketId);
            if (reasult == null)
            {
                return NotFound(new { message = $"Ticket with Id {ticketId} not found" });
            }
            return NoContent();
        }
    }
}
