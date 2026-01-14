using server.Models;
using System.Globalization;

namespace server.Interfaces
{
    public interface IPackageRepository
    {
        Task<IEnumerable<Package>> GetAll();
        Task<Package> GetById(int id);
        Task<Package> AddPackage(Package package);
        Task<Package> UpdatePackage(Package package);
        Task<bool> DeletePackage(int id);
        Task<IEnumerable<Package>> SortPackages(string? sortBy);
    }
}
