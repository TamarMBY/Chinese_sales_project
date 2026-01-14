using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Repositories;

namespace server.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly ILogger<PackageService> _logger;
        public PackageService(IPackageRepository packageRepository, ILogger<PackageService> logger)
        {
            _packageRepository = packageRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<PackageResponseDto>> GetAll()
        {
            _logger.LogInformation("Get/ get all packages called");
            try
            {
                var packages = await _packageRepository.GetAll();
                return packages.Select(MapToResponeseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all packages");
                throw;
            }
        }

        public async Task<PackageResponseDto> GetById(int id)
        {
            _logger.LogInformation("Get/ get package by id: {packageId}", id);
            try
            {
                var package = await _packageRepository.GetById(id);
                return package != null ? MapToResponeseDto(package) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching package with Id {PackageId}", id);
                throw;
            }
        }
        public async Task<PackageResponseDto> AddPackage(PackageCreateDto packagDto)
        {
            _logger.LogInformation("Post/ add package called");
            var package = new Package
            {
                Name = packagDto.Name,
                Description = packagDto.Description,
                Quantity = packagDto.Quantity,
                Price = packagDto.Price
            };
            try
            {
                var createPackage = await _packageRepository.AddPackage(package);
                _logger.LogInformation("Package created successfully with Id {PackageId}", createPackage.Id);
                return MapToResponeseDto(createPackage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding package Name={Name}", packagDto.Name);
                throw;
            }
        }

        public async Task<bool> DeletePackage(int id)
        {
            _logger.LogInformation("Delete/ delete package called");
            try
            {
                return await _packageRepository.DeletePackage(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting package with Id {PackageId}", id);
                throw;
            }
        }

        public async Task<PackageResponseDto> UpdatePackage(int packageId, PackageUpdateDto packageDto)
        {
            _logger.LogInformation("Put/ update package called");
            try
            {
                var existingPackage = await _packageRepository.GetById(packageId);
                if (existingPackage == null)
                    return null;
                existingPackage.Name = packageDto.Name;
                existingPackage.Description = packageDto.Description;
                existingPackage.Quantity = packageDto.Quantity;
                existingPackage.Price = packageDto.Price;
                var updatedCategory = await _packageRepository.UpdatePackage(existingPackage);
                _logger.LogInformation("Package with Id {PackageId} updated successfully", packageId);
                return MapToResponeseDto(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating package with Id {PackageId}", packageId);
                throw;
            }
        }
        public async Task<IEnumerable<PackageResponseDto>> SortPackages(string? sortBy)
        {
            _logger.LogInformation("Get/ sort packages called");
            try
            {
                var packeges = await _packageRepository.SortPackages(sortBy);
                return packeges.Select(MapToResponeseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sorting packages SortBy={SortBy}", sortBy);
                throw;
            }
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
