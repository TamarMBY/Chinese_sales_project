using server.DTOs;
using server.Models;
using System.Threading.Tasks;

namespace server.Interfaces
{
    public interface IGiftService
    {
        Task<IEnumerable<GiftRespnseDto>> GetAll();
        Task<GiftRespnseDto> GetById(int id);
        Task<GiftRespnseDto> AddGift(GiftCreateDto gift);
        Task<GiftRespnseDto> UpdateGift(int giftId,GiftUpdateDto gift);
        Task<bool> DeleteGift(int id);
        Task<GiftRespnseDto> Lottery(int giftId);
        Task<IEnumerable<GiftRespnseDto>> FilterGifts(string? giftName, int? categoryId, string? donorName, int? buyersCount);
    }
}
