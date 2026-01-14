using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;
using System.Net.Sockets;

namespace server.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ApplicationDbContext _context;
        public PurchaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Purchase>> GetAll()
        {
            return await _context.Purchases
                 .Include(b => b.Buyer)
                 .ToListAsync();
        }
        public async Task<Purchase> GetById(int id)
        {
            return await _context.Purchases
                .Include(b => b.Buyer)
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Purchase> GetByUserId(string userId)
        {
            return await _context.Purchases
                .Include(b => b.Buyer)
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(p => p.BuyerId == userId);
        }
        public async Task<Purchase> AddPurchase(Purchase purchase)
        {
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }

        public async Task<bool> DeletePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null) return false;
            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Purchase> UpdatePurchase(Purchase purchase)
        {
            _context.Purchases.Update(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }

        public async Task<Purchase>AddPackageToPurchase(int purchaseId, Package package)
        {
            var purchase = await GetById(purchaseId);
            if (purchase == null) return null;
            purchase.Packages.Add(package);
            await _context.SaveChangesAsync();
            return purchase;
        }

        public async Task<Purchase> DeletePackageFromPurchase(int purchaseId, int packageId)
        {
            var purchase = await GetById(purchaseId);
            if (purchase == null) return null;
            var existing = purchase.Packages.FirstOrDefault(p => p.Id == packageId);
            if (existing == null) return purchase;
            purchase.Packages.Remove(existing);
            await _context.SaveChangesAsync();
            return purchase;
        }
        public async Task<Purchase> AddTicketToPurchase(int purchaseId, Ticket ticket)
        {
            var purchase = await GetById(purchaseId);
            if (purchase == null) return null;
            purchase.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return purchase;
        }
        public async Task<Purchase> DeleteTicket(int purchaseId, int ticketId)
        {
            var purchase = await GetById(purchaseId);
            if (purchase == null) return null;
            var existing = purchase.Tickets.FirstOrDefault(t => t.Id == ticketId);
            if(existing == null) return purchase;
            purchase.Tickets.Remove(existing);
            await _context.SaveChangesAsync();
            return purchase;
        }
    }
}
