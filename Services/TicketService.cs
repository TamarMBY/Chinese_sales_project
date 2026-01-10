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

        public TicketService(
            ITicketRepository ticketRepository, IPurchaseService purchaseService, IGiftService gifService
            )
        {
            _ticketRepository = ticketRepository;
            _purchaseService = purchaseService;
            _gifService = gifService;
        }
        public async Task<IEnumerable<TicketRespnseDto>> GetAll()
        {
            var tickets = await _ticketRepository.GetAll();
            return tickets.Select(MapToResponeseDto);
        }
        public async Task<TicketRespnseDto> GetById(int id)
        {
            var ticket = await _ticketRepository.GetById(id);
            return ticket != null ? MapToResponeseDto(ticket) : null;
        }

        public async Task<TicketRespnseDto> AddTicket(TicketCreateDto ticketDto)
        {
            var gift = await _gifService.GetById(ticketDto.GiftId);
            if(gift.IsDrawn)
                throw new InvalidOperationException("This gift already drawn");
            var newTicket = new Ticket
            {
                PurchaseId = ticketDto.PurchaseId,
                GiftId = ticketDto.GiftId
            };
            var createdTicket = await _ticketRepository.AddTicket(newTicket);
            await _purchaseService.AddTicketToPurchase(ticketDto.PurchaseId, createdTicket);
            return MapToResponeseDto(createdTicket);
        }

        public async Task<bool> DeleteTicket(int id)
        {
            return await _ticketRepository.DeleteTicket(id);
        }

        public async Task<TicketRespnseDto> UpdateTicket(int ticketId, TicketUpdateDto ticketDto)
        {
            var existingTicket = await _ticketRepository.GetById(ticketId);
            if (existingTicket == null)
                return null;
            existingTicket.PurchaseId = ticketDto.PurchaseId;
            existingTicket.GiftId = ticketDto.GiftId;
            existingTicket.Quantity = ticketDto.Quantity;
            var updatedTicket = await _ticketRepository.UpdateTicket(existingTicket);
            return MapToResponeseDto(updatedTicket);
        }

        private static TicketRespnseDto MapToResponeseDto(Ticket ticket)
        {
            return new TicketRespnseDto
            {
                Id = ticket.Id,
                PurchaseId=ticket.PurchaseId,
                GiftId=ticket.GiftId,
                Quantity = ticket.Quantity,
            };
        }
    }
}
