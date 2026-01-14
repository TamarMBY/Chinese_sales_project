using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Interfaces;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketRespnseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TicketRespnseDto>>> GetAll()
        {
            var tickets = await _ticketService.GetAll();
            return Ok(tickets);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketRespnseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetById(id);
            if (ticket == null)
            {
                return NotFound(new { message = $"Ticket with ID {id} not exist" });
            }
            return Ok(ticket);
        }
        [HttpPost]
        [ProducesResponseType(typeof(TicketRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddTicket([FromBody] TicketCreateDto createDto)
        {
            try
            {
                var ticket = await _ticketService.AddTicket(createDto);
                return Ok(ticket);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(TicketRespnseDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var reasult = await _ticketService.DeleteTicket(id);
            if (!reasult)
            {
                return NotFound(new { message = $"Ticket with Id {id} not found" });
            }
            return NoContent();
        }
        [HttpPut("{ticketId}")]
        [ProducesResponseType(typeof(TicketRespnseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateTicket([FromRoute] int ticketId,[FromBody] TicketUpdateDto updateDto)
        {
            try
            {
                var ticket = await _ticketService.UpdateTicket(ticketId,updateDto);
                return Ok(ticket);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { messege = ex.Message });
            }
        }
    }
}
