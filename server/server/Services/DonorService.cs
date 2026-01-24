using server.DTOs;
using server.Interfaces;
using server.Models;

namespace server.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorRepository;
        private readonly ILogger<DonorService> _logger;
        public DonorService(IDonorRepository donorRepository, ILogger<DonorService> logger)
        {
            _donorRepository = donorRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<DonorRespnseDto>> GetAll()
        {
            _logger.LogInformation("Get/ get all donors called");
            try
            {
                var donors = await _donorRepository.GetAll();
                return donors.Select(MapToResponeseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all donors");
                throw;
            }
            
        }
        public async Task<DonorRespnseDto> GetById(string id)
        {
            _logger.LogInformation("Get/ get donor by id: {donorId}", id);
            try
            {
                var donor = await _donorRepository.GetById(id);
                return donor != null ? MapToResponeseDto(donor) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching donor with Id {DonorId}", id);
                throw;
            }
        }
        public async Task<DonorRespnseDto> AddDonor(DonorCreateDto donorDto)
        {
            _logger.LogInformation("Post/ add donor called");
            var newDonor = new Donor
            {
                Id = donorDto.Id,
                Name = donorDto.Name,
                PhoneNumber = donorDto.PhoneNumber,
                Email = donorDto.Email,
                LogoUrl = donorDto.LogoUrl
            };
            try
            {
                var createdDonor = await _donorRepository.AddDonor(newDonor);
                _logger.LogInformation("Donor created successfully with Id {DonorId}", createdDonor.Id);
                return MapToResponeseDto(createdDonor);
            }
            catch (Exception ex)
            {
                _logger.LogError("\"Error while adding donor Name={Name}", donorDto.Name);
                throw;
            }

            
        }

        public async Task<bool> DeleteDonor(string id)
        {
            _logger.LogInformation("Delete/ delete donor called");
            try
            {
                return await _donorRepository.DeleteDonor(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting donor with Id {DonorId}", id);
                throw;
            }            
        }

        public async Task<DonorRespnseDto> UpdateDonor(string donorId,DonorUpdateDto donorDto)
        {
            _logger.LogInformation("Put/ update donor called");
            try
            {
                var existingDonor = await _donorRepository.GetById(donorId);
                if (existingDonor == null)
                    return null;
                existingDonor.Name = donorDto.Name;
                existingDonor.PhoneNumber = donorDto.PhoneNumber;
                existingDonor.Email = donorDto.Email;
                existingDonor.LogoUrl = donorDto.LogoUrl;
                var updatedDonor = await _donorRepository.UpdateDonor(existingDonor);
                _logger.LogInformation("Donor with Id {DonorId} updated successfully", donorId);
                return MapToResponeseDto(updatedDonor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating donor with Id {DonorId}", donorId);
                throw;
            }
           
        }
        public async Task<IEnumerable<DonorRespnseDto>> FilterDonors(string? name, string? email, int? giftId)
        {
            _logger.LogInformation("Get/ filter donors called");
            try
            {
                var donors = await _donorRepository.FilterDonors(name, email, giftId);
                return donors.Select(MapToResponeseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while filtering donors Name={Name}, Email={Email}, GiftId={GiftId}",
                    name,
                    email,
                    giftId
                );
                throw;
            }
        }

        private static DonorRespnseDto MapToResponeseDto(Donor donor)
        {
            return new DonorRespnseDto
            {
                Id = donor.Id,
                Name = donor.Name,
                PhoneNumber = donor.PhoneNumber,
                Email = donor.Email,
                LogoUrl = donor.LogoUrl
            };
        }
    }
}
