using server.Models;

namespace server.Interfaces
{
    public interface IGiftRepository
    {
        Task<IEnumerable<Gift>> GetAll();
        Task<Gift> GetById(int id);
        Task<Gift> AddGift(Gift gift);
        Task<Gift> UpdateGift(Gift gift);
        Task<bool> DeleteGift(int id);
        Task<IEnumerable<Gift>> FilterGifts(string? giftName, string? donorName, int? buyersCount);

    }
}
