using server.DTOs;

namespace server.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketRespnseDto>> GetAll();
        Task<TicketRespnseDto> GetById(int id);
        Task<TicketRespnseDto> AddTicket(TicketCreateDto ticket);
        Task<TicketRespnseDto> UpdateTicket(int ticketId,TicketUpdateDto ticket);
        Task<bool> DeleteTicket(int id);
    }
}
