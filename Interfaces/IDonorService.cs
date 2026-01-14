using server.DTOs;
using server.Models;

namespace server.Interfaces
{
    public interface IDonorService
    {
        Task<IEnumerable<DonorRespnseDto>> GetAll();
        Task<DonorRespnseDto> GetById(string id);
        Task<DonorRespnseDto> AddDonor(DonorCreateDto donor);
        Task<DonorRespnseDto> UpdateDonor(string donorId, DonorUpdateDto donor);
        Task<bool> DeleteDonor(string id);
        Task<IEnumerable<DonorRespnseDto>> FilterDonors(string? name, string? email, int? giftId);

    }
}
