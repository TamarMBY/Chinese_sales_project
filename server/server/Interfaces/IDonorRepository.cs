using server.Models;

namespace server.Interfaces
{
    public interface IDonorRepository
    {
        Task<IEnumerable<Donor>> GetAll();
        Task<Donor> GetById(string id);
        Task<Donor> AddDonor(Donor donor);
        Task<Donor> UpdateDonor(Donor donor);
        Task<bool> DeleteDonor(string id);
        Task<IEnumerable<Donor>> FilterDonors(string? name, string? email, int? giftId);
    }
}
