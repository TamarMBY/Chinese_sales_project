using server.Models;

namespace server.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<IEnumerable<Purchase>> GetAll();
        Task<Purchase> GetById(int id);
        Task<Purchase> GetByUserId(string id);
        Task<Purchase> AddPurchase(Purchase purchase);
        Task<Purchase> UpdatePurchase(Purchase purchase);
        Task<bool> DeletePurchase(int id);
        Task<Purchase> AddPackageToPurchase(int purchaseId, Package package);
        Task<Purchase> DeletePackageFromPurchase(int purchaseId, int packageId);
        Task<Purchase> AddTicketToPurchase(int purchaseId, Ticket ticket);
        Task<Purchase> DeleteTicket (int purchaseId, int ticketId);
    }
}
