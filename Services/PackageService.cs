using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Repositories;

namespace server.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }
        public async Task<IEnumerable<PackageResponseDto>> GetAll()
        {
            var packages = await _packageRepository.GetAll();
            return packages.Select(MapToResponeseDto);
        }

        public async Task<PackageResponseDto> GetById(int id)
        {
            var package = await _packageRepository.GetById(id);
            return package != null ? MapToResponeseDto(package) : null;
        }
        public async Task<PackageResponseDto> AddPackage(PackageCreateDto packagDto)
        {
            var package = new Package
            {
                Name = packagDto.Name,
                Description = packagDto.Description,
                Quantity = packagDto.Quantity,
                Price = packagDto.Price
            };
            var createPackage = await _packageRepository.AddPackage(package);
            return MapToResponeseDto(createPackage);
        }

        public async Task<bool>DeletePackage(int id)
        {
            return await _packageRepository.DeletePackage(id);
        }

        public async Task<PackageResponseDto>UpdatePackage(int packageId, PackageUpdateDto packageDto)
        {
            var existingPackage = await _packageRepository.GetById(packageId);
            if (existingPackage == null)
                return null;
            existingPackage.Name = packageDto.Name;
            existingPackage.Description = packageDto.Description;
            existingPackage.Quantity = packageDto.Quantity;
            existingPackage.Price = packageDto.Price;
            var updatedCategory = await _packageRepository.UpdatePackage(existingPackage);
            return MapToResponeseDto(updatedCategory);
        }
        public async Task<IEnumerable<PackageResponseDto>> SortPackages(string? sortBy)
        {
            var packeges = await _packageRepository.SortPackages(sortBy);
            return packeges.Select(MapToResponeseDto);
        }

        private static PackageResponseDto MapToResponeseDto(Package package)
        {
            return new PackageResponseDto
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Quantity = package.Quantity,
                Price = package.Price
            };
        }
    }
}
