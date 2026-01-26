using server.DTOs;
using server.Models;
using System.Threading.Tasks;

namespace server.Interfaces
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchaseRespnseDto>> GetAll();
        Task<PurchaseRespnseDto> GetById(int id);
        Task<PurchaseRespnseDto> GetByUserId(string userId);
        Task<PurchaseRespnseDto> AddPurchase(PurchaseCreateDto purchase);
        Task<PurchaseRespnseDto> UpdatePurchase(int purchaseId,PurchaseUpdateDto purchase);
        Task<bool> DeletePurchase(int id);
        Task<PurchaseRespnseDto> AddPackageToPurchase(int purchaseId, Package package);
        Task<PurchaseRespnseDto> DeletePackageFromPurchase(int purchaseId, int packageId);
        Task<PurchaseRespnseDto> AddTicketToPurchase(TicketCreateDto tCrateDto);
        Task<PurchaseRespnseDto> DeleteTicket(int purchaseId, int ticketId);
    }
}
