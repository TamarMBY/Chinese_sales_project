using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Interfaces;
using server.Models;

namespace server.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly ApplicationDbContext _context;
        public PackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Package>> GetAll()
        {
            return await _context.Packages
                           .ToListAsync();
        }

        public async Task<Package> GetById(int id)
        {
            return await _context.Packages
                           .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Package> AddPackage(Package package)
        {
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task<bool> DeletePackage(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null) return false;
            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Package> UpdatePackage(Package package)
        {
            _context.Packages.Update(package);
            await _context.SaveChangesAsync();
            return package;
        }
        public async Task<IEnumerable<Package>> SortPackages(string? sortBy)
        {
            var query = _context.Packages
                .Include(p => p.Purchases)
                .AsQueryable();
            query = sortBy switch
            {
                "price_desc" => query.OrderByDescending(p => p.Price),
                "most_purchased" => query.OrderByDescending(
                         p => p.Purchases.Count(pr => !pr.IsDraft)),
                         _ => query.OrderBy(p => p.Id)
            };
            return await query.ToListAsync();
        }
    }
}
