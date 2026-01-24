using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.DTOs;
using server.Interfaces;
using server.Models;

namespace server.Repositories
{
    public class GiftRepository : IGiftRepository
    {
        private readonly ApplicationDbContext _context;
        public GiftRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Gift>> GetAll()
        {
            return await _context.Gifts
                         .ToListAsync();
        }
        public async Task<Gift> GetById(int id)
        {
            return await _context.Gifts
            .Where(g => g.Id == id)
            .Include(g => g.Tickets.Where(t => !t.Purchase.IsDraft))
                .ThenInclude(t => t.Purchase)
            .FirstOrDefaultAsync();
        }
        public async Task<Gift> AddGift(Gift gift)
        {
            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();
            return gift;
        }
        public async Task<bool> DeleteGift(int id)
        {
            var gift = await _context.Gifts.FindAsync(id);
            if (gift == null) return false;
            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Gift> UpdateGift(Gift gift)
        {
            _context.Gifts.Update(gift);
            await _context.SaveChangesAsync();
            return gift;
        }
        public async Task<IEnumerable<Gift>> FilterGifts(string? giftName, int?categoryId, string? donorName, int? buyersCount)
        {
            var query = _context.Gifts
                .Include(g => g.Donor)
                .Include(g  => g.Tickets)
                    .ThenInclude(t=>t.Purchase)
                .AsQueryable();
            if (!string.IsNullOrEmpty(giftName))
            {
                query = query.Where(g => g.Name.Contains(giftName));
            }
            if(categoryId.HasValue)
            {
                query = query.Where(g =>
                    g.CategoryId == categoryId
                );
            }
            if (!string.IsNullOrEmpty(donorName))
            {
                query = query.Where(g => g.Donor.Name.Contains(donorName));
            }
            if (buyersCount.HasValue)
            {
                query = query.Where(g =>
                    g.Tickets
                        .Where(t => !t.Purchase.IsDraft)
                        .Select(t => t.PurchaseId)
                        .Distinct()
                        .Count() == buyersCount.Value
                );
            }
            return await query.ToListAsync();
        }
    }
}
