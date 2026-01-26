using server.Data;
using server.Models;
using Microsoft.EntityFrameworkCore;
using server.Interfaces;
using server.DTOs;


namespace server.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;
        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAll()
        {
            return await _context.Tickets
                         .ToListAsync();
        }
        public async Task<Ticket> GetById(int id)
        {
            return await _context.Tickets
                       .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Ticket> AddTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return false;
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Ticket> UpdateTicket(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        Task<Ticket> ITicketRepository.AddTicket(TicketCreateDto tCreateDto)
        {
            throw new NotImplementedException();
        }
    }
}
