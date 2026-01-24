using server.Models;

namespace server.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAll();
        Task<Ticket> GetById(int id);
        Task<Ticket> AddTicket(Ticket ticket);
        Task<Ticket> UpdateTicket(Ticket ticket);
        Task<bool> DeleteTicket(int id);
    }
}
