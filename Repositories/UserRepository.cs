using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;

namespace server.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>>GetAll()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User>GetById(string id)
        {
            return await _context.Users
                .Include(u => u.Purchases)
                .FirstOrDefaultAsync();
        }
    }
}
