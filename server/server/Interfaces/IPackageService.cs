using server.DTOs;
using server.Models;

namespace server.Interfaces
{
    public interface IPackageService
    {
        Task<IEnumerable<PackageResponseDto>> GetAll();
        Task<PackageResponseDto> GetById(int id);
        Task<PackageResponseDto> AddPackage(PackageCreateDto package);
        Task<PackageResponseDto> UpdatePackage(int packageId,PackageUpdateDto package);
        Task<bool> DeletePackage(int id);
        Task<IEnumerable<PackageResponseDto>> SortPackages(string? sortBy);

    }
}
