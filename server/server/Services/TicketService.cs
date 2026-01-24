using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Repositories;
using System.Net.Sockets;

namespace server.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IPurchaseService _purchaseService;
        private readonly IGiftService _gifService;
        private readonly ILogger<TicketService> _logger;

        public TicketService(
            ITicketRepository ticketRepository, IPurchaseService purchaseService, IGiftService gifService, ILogger<TicketService> logger
            )
        {
            _ticketRepository = ticketRepository;
            _purchaseService = purchaseService;
            _gifService = gifService;
            _logger = logger;
        }
        public async Task<IEnumerable<TicketRespnseDto>> GetAll()
        {
            _logger.LogInformation("Get/ get all tickets called");
            try
            {
                var tickets = await _ticketRepository.GetAll();
                return tickets.Select(MapToResponeseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all tickets");
                throw;
            }
        }
        public async Task<TicketRespnseDto> GetById(int id)
        {
            _logger.LogInformation("Get/ get ticket by id: {ticketId}", id);
            try
            {
                var ticket = await _ticketRepository.GetById(id);
                return ticket != null ? MapToResponeseDto(ticket) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ticket with Id {TicketId}", id);
                throw;
            }
        }

        public async Task<TicketRespnseDto> AddTicket(TicketCreateDto ticketDto)
        {
            _logger.LogInformation("Post/ add ticket called");
            try
            {
                var gift = await _gifService.GetById(ticketDto.GiftId);
                if (gift.IsDrawn)
                    throw new InvalidOperationException("This gift already drawn");
                var newTicket = new Ticket
                {
                    PurchaseId = ticketDto.PurchaseId,
                    GiftId = ticketDto.GiftId
                };
                var createdTicket = await _ticketRepository.AddTicket(newTicket);
                await _purchaseService.AddTicketToPurchase(ticketDto.PurchaseId, createdTicket);
                _logger.LogInformation("Ticket created successfully with Id {TicketId}", createdTicket.Id);
                return MapToResponeseDto(createdTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding ticket GiftId={GiftId}", ticketDto.GiftId);
                throw;
            }
        }

        public async Task<bool> DeleteTicket(int id)
        {
            _logger.LogInformation("Delete/ delete ticket called");
            try
            {
                return await _ticketRepository.DeleteTicket(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting ticket with Id {TicketId}", id);
                throw;
            }
        }

        public async Task<TicketRespnseDto> UpdateTicket(int ticketId, TicketUpdateDto ticketDto)
        {
            _logger.LogInformation("Put/ update ticket called");
            try
            {
                var existingTicket = await _ticketRepository.GetById(ticketId);
                if (existingTicket == null)
                    return null;
                existingTicket.PurchaseId = ticketDto.PurchaseId;
                existingTicket.GiftId = ticketDto.GiftId;
                existingTicket.Quantity = ticketDto.Quantity;
                var updatedTicket = await _ticketRepository.UpdateTicket(existingTicket);
                _logger.LogInformation("Ticket with Id {TicketId} updated successfully", ticketId);
                return MapToResponeseDto(updatedTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating ticket with Id {TicketId}", ticketId);
                throw;
            }
        }

        private static TicketRespnseDto MapToResponeseDto(Ticket ticket)
        {
            return new TicketRespnseDto
            {
                Id = ticket.Id,
                PurchaseId = ticket.PurchaseId,
                GiftId = ticket.GiftId,
                Quantity = ticket.Quantity,
            };
        }
    }
}
