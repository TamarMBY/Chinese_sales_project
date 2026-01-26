using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Repositories;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace server.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ITicketService _ticketService;
        private readonly ILogger<PurchaseService> _logger;


        public PurchaseService(IPurchaseRepository purchaseRepository, ITicketService ticketService, ILogger<PurchaseService> logger)
        {
            _purchaseRepository = purchaseRepository;
            _ticketService = ticketService;
            _logger = logger;
        }
        public async Task<IEnumerable<PurchaseRespnseDto>> GetAll()
        {
            _logger.LogInformation("Get/ get all purchases called");
            try
            {
                var purchases = await _purchaseRepository.GetAll();
                return purchases.Select(MapToResponeseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all purchases");
                throw;
            }
        }
        public async Task<PurchaseRespnseDto> GetById(int id)
        {
            _logger.LogInformation("Get/ get purchase by id: {purchaseId}", id);
            try
            {
                var purchase = await _purchaseRepository.GetById(id);
                return purchase != null ? MapToResponeseDto(purchase) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching purchase with Id {PurchaseId}", id);
                throw;
            }
        }
        public async Task<PurchaseRespnseDto> GetByUserId(string userId)
        {
            _logger.LogInformation("Get/ get purchase by user id: {userId}", userId);
            try
            {
                var purchase = await _purchaseRepository.GetByUserId(userId);
                return purchase != null ? MapToResponeseDto(purchase) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching purchase by user id {UserId}", userId);
                throw;
            }
        }

        public async Task<PurchaseRespnseDto> AddPurchase(PurchaseCreateDto purchaseDto)
        {
            _logger.LogInformation("Post/ add purchase called");
            var newPurchase = new Purchase
            {
                BuyerId = purchaseDto.BuyerId,
                TotalAmount = purchaseDto.TotalAmount,
                OrderDate = purchaseDto.OrderDate,
            };
            try
            {
                var createdPurchase = await _purchaseRepository.AddPurchase(newPurchase);
                _logger.LogInformation("Purchase created successfully with Id {PurchaseId}", createdPurchase.Id);
                return MapToResponeseDto(createdPurchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding purchase BuyerId={BuyerId}", purchaseDto.BuyerId);
                throw;
            }
        }

        public async Task<bool> DeletePurchase(int id)
        {
            _logger.LogInformation("Delete/ delete purchase called");
            try
            {
                return await _purchaseRepository.DeletePurchase(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting purchase with Id {PurchaseId}", id);
                throw;
            }
        }

        public async Task<PurchaseRespnseDto> UpdatePurchase(int purchaseId, PurchaseUpdateDto purchaseDto)
        {
            _logger.LogInformation("Put/ update purchase called");
            try
            {
                var existingPurchase = await _purchaseRepository.GetById(purchaseId);
                if (existingPurchase == null)
                    return null;
                existingPurchase.BuyerId = purchaseDto.BuyerId;
                existingPurchase.TotalAmount = purchaseDto.TotalAmount;
                existingPurchase.OrderDate = purchaseDto.OrderDate;
                existingPurchase.IsDraft = purchaseDto.IsDraft;
                var updatedPurchase = await _purchaseRepository.UpdatePurchase(existingPurchase);
                _logger.LogInformation("Purchase with Id {PurchaseId} updated successfully", purchaseId);
                return MapToResponeseDto(updatedPurchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating purchase with Id {PurchaseId}", purchaseId);
                throw;
            }
        }

        public async Task<PurchaseRespnseDto> AddPackageToPurchase(int purchaseId, Package package)
        {
            _logger.LogInformation("Post/ add package to purchase called");
            try
            {
                var purchase = await _purchaseRepository.GetById(purchaseId);
                if (purchase == null) return null;
                if (!purchase.IsDraft)
                    throw new InvalidOperationException("Cannot modify a finalized purchase.");
                var updated = await _purchaseRepository.AddPackageToPurchase(purchaseId, package);
                _logger.LogInformation("Package added to purchase Id {PurchaseId} successfully", purchaseId);
                return MapToResponeseDto(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding package to purchase Id {PurchaseId}", purchaseId);
                throw;
            }
        }

        public async Task<PurchaseRespnseDto> DeletePackageFromPurchase(int purchaseId, int packageId)
        {
            _logger.LogInformation("Delete/ delete package from purchase called");
            try
            {
                var purchase = await _purchaseRepository.GetById(purchaseId);
                if (purchase == null) return null;
                if (!purchase.IsDraft)
                    throw new InvalidOperationException("Cannot modify a finalized purchase.");
                var deleted = await _purchaseRepository.DeleteTicket(purchaseId, packageId);
                _logger.LogInformation("Package deleted from purchase Id {PurchaseId} successfully", purchaseId);
                return MapToResponeseDto(deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting package {PackageId} from purchase {PurchaseId}", packageId, purchaseId);
                throw;
            }
        }
        public async Task<PurchaseRespnseDto> AddTicketToPurchase(TicketCreateDto tCreateDto)
        {
            _logger.LogInformation("Post/ add ticket to purchase called");
            try
            {
                var purchase = await _purchaseRepository.GetById(tCreateDto.PurchaseId);
                if (purchase == null) return null;
                if (!purchase.IsDraft)
                    throw new InvalidOperationException("Cannot modify a finalized purchase.");
                if (GetRemainingTicketsCount(purchase) <= 0)
                {
                    throw new InvalidOperationException("You need to buy a new package.");
                }
                var ticket = await _ticketService.AddTicket(tCreateDto);
                var existingTicket = purchase.Tickets
                    .FirstOrDefault(t => t.GiftId == ticket.GiftId);
                if(existingTicket != null)
                {
                    existingTicket.Quantity++;
                    await _ticketService.UpdateTicket(existingTicket.Id, new TicketUpdateDto
                    {
                        GiftId = existingTicket.GiftId,
                        PurchaseId = existingTicket.PurchaseId,
                        Quantity = existingTicket.Quantity
                    });
                    return MapToResponeseDto(purchase);
                }
                var updated = await _purchaseRepository.AddTicketToPurchase(new Ticket
                {
                    Id = ticket.Id,
                    GiftId = ticket.GiftId,
                    PurchaseId = ticket.PurchaseId,
                    Quantity = ticket.Quantity
                });
                _logger.LogInformation("Ticket added to purchase Id {PurchaseId} successfully", tCreateDto.PurchaseId);
                return MapToResponeseDto(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding ticket to purchase {PurchaseId}", tCreateDto.PurchaseId);
                throw;
            }
        }
        public async Task<PurchaseRespnseDto> DeleteTicket(int purchaseId, int ticketId)
        {
            _logger.LogInformation("Delete/ delete ticket from purchase called");
            try
            {
                var purchase = await _purchaseRepository.GetById(purchaseId);
                if (purchase == null) return null;
                if (!purchase.IsDraft)
                    throw new InvalidOperationException("Cannot modify a finalized purchase.");
                var deleted = await _purchaseRepository.DeleteTicket(purchaseId, ticketId);
                _logger.LogInformation("Ticket deleted from purchase Id {PurchaseId} successfully", purchaseId);
                return MapToResponeseDto(deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting ticket {TicketId} from purchase {PurchaseId}", ticketId, purchaseId);
                throw;
            }
        }

        private static PurchaseRespnseDto MapToResponeseDto(Purchase purchase)
        {
            return new PurchaseRespnseDto
            {
                Id = purchase.Id,
                BuyerId = purchase.BuyerId,
                TotalAmount = purchase.TotalAmount,
                OrderDate = purchase.OrderDate,
                IsDraft = purchase.IsDraft,
            };
        }

        private static int GetRemainingTicketsCount(Purchase purchase)
        {
            var packageCount = purchase.Packages.Sum(pkg => pkg.Quantity);
            var ticketCount = purchase.Tickets.Sum(t => t.Quantity);
            return packageCount - ticketCount;
        }


    }
}
