using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Repositories
{
    public class DonorRepository : IDonorRepository
    {
        private readonly ApplicationDbContext _context;
        public DonorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Donor>> GetAll()
        {
            return await _context.Donors
                .Include(d => d.Gifts)
                .ToListAsync();
        }
        public async Task<Donor> GetById(string id)
        {
            return await _context.Donors
                .Include(d => d.Gifts)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Donor> AddDonor(Donor donor)
        {
            _context.Donors.Add(donor);
            await _context.SaveChangesAsync();
            return donor;
        }

        public async Task<bool> DeleteDonor(string id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null) return false;
            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Donor> UpdateDonor(Donor donor)
        {
            _context.Donors.Update(donor);
            await _context.SaveChangesAsync();
            return donor;
        }

        public async Task<IEnumerable<Donor>> FilterDonors(string? name, string? email, int? giftId)
        {
            var query = _context.Donors
                .Include(d => d.Gifts)
                .AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(d => d.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(d => d.Email.Contains(email));
            }
            if (giftId.HasValue)
            {
                query = query.Where(d => d.Gifts.Any(g => g.Id == giftId.Value));
            }

            return await query.ToListAsync();
        }
    }
}
