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


        public PurchaseService(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }
        public async Task<IEnumerable<PurchaseRespnseDto>> GetAll()
        {
            var purchases = await _purchaseRepository.GetAll();
            return purchases.Select(MapToResponeseDto);
        }
        public async Task<PurchaseRespnseDto> GetById(int id)
        {
            var purchase = await _purchaseRepository.GetById(id);
            return purchase != null ? MapToResponeseDto(purchase) : null;
        }
        public async Task<PurchaseRespnseDto> GetByUserId(string userId)
        {
            var purchase = await _purchaseRepository.GetByUserId(userId);
            return purchase != null ? MapToResponeseDto(purchase) : null;
        }

        public async Task<PurchaseRespnseDto> AddPurchase(PurchaseCreateDto purchaseDto)
        {
            var newPurchase = new Purchase
            {
                BuyerId = purchaseDto.BuyerId,
                TotalAmount = purchaseDto.TotalAmount,
                OrderDate = purchaseDto.OrderDate,
            };

            var createdPurchase = await _purchaseRepository.AddPurchase(newPurchase);
            return MapToResponeseDto(createdPurchase);
        }

        public async Task<bool> DeletePurchase(int id)
        {
            return await _purchaseRepository.DeletePurchase(id);
        }

        public async Task<PurchaseRespnseDto> UpdatePurchase(int purchaseId, PurchaseUpdateDto purchaseDto)
        {
            var existingPurchase = await _purchaseRepository.GetById(purchaseId);
            if (existingPurchase == null)
                return null;
            existingPurchase.BuyerId = purchaseDto.BuyerId;
            existingPurchase.TotalAmount = purchaseDto.TotalAmount;
            existingPurchase.OrderDate = purchaseDto.OrderDate;
            existingPurchase.IsDraft = purchaseDto.IsDraft;
            var updatedPurchase = await _purchaseRepository.UpdatePurchase(existingPurchase);
            return MapToResponeseDto(updatedPurchase);
        }

        public async Task<PurchaseRespnseDto> AddPackageToPurchase(int purchaseId, Package package)
        {
            var purchase = await _purchaseRepository.GetById(purchaseId);
            if (purchase == null) return null;
            if (!purchase.IsDraft)
                throw new InvalidOperationException("Cannot modify a finalized purchase.");
            var updated = await _purchaseRepository.AddPackageToPurchase(purchaseId, package);
            return MapToResponeseDto(updated);
        }

        public async Task<PurchaseRespnseDto> DeletePackageFromPurchase(int purchaseId, int packageId)
        {
            var purchase = await _purchaseRepository.GetById(purchaseId);
            if (purchase == null) return null;
            if (!purchase.IsDraft)
                throw new InvalidOperationException("Cannot modify a finalized purchase.");
            var deleted = await _purchaseRepository.DeleteTicket(purchaseId, packageId);
            return MapToResponeseDto(deleted);
        }
        public async Task<PurchaseRespnseDto> AddTicketToPurchase(int purchaseId, Ticket ticket)
        {
            var purchase = await _purchaseRepository.GetById(purchaseId);
            if (purchase == null) return null;
            if(!purchase.IsDraft)
                throw new InvalidOperationException("Cannot modify a finalized purchase.");
            if(GetRemainingTicketsCount(purchase) <= 0)
            {
                throw new InvalidOperationException("You need to buy a new package.");
            }
            if (purchase.Tickets.Any(t => t.Id == ticket.Id))
            {
                ticket.Quantity++;
                return MapToResponeseDto(purchase);
            }
            var updated = await _purchaseRepository.AddTicketToPurchase(purchaseId, ticket);
            return MapToResponeseDto(updated);
        }
        public async Task<PurchaseRespnseDto> DeleteTicket(int purchaseId, int ticketId)
        {
            var purchase = await _purchaseRepository.GetById(purchaseId);
            if (purchase == null) return null;
            if (!purchase.IsDraft)
                throw new InvalidOperationException("Cannot modify a finalized purchase.");
            var deleted = await _purchaseRepository.DeleteTicket(purchaseId, ticketId);
            return MapToResponeseDto(deleted);
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
