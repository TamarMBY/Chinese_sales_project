using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Repositories;
using System.Text;

namespace server.Services
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _giftRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ILogger<GiftService> _logger;
        public GiftService(IGiftRepository giftRepository,IPurchaseRepository purchaseRepository, ILogger<GiftService> logger)
        {
            _giftRepository = giftRepository;
            _purchaseRepository = purchaseRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<GiftRespnseDto>> GetAll()
        {
            _logger.LogInformation("Get/ get all gifts called");
            try
            {
                var gifts = await _giftRepository.GetAll();
                return gifts.Select(MapToResponeseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all gifts");
                throw;
            }
        }
        public async Task<GiftRespnseDto> GetById(int id)
        {
            _logger.LogInformation("Get/ get gift by id: {giftId}", id);
            try
            {
                var gift = await _giftRepository.GetById(id);
                return gift != null ? MapToResponeseDto(gift) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching gift with Id {GiftId}", id);
                throw;
            }
        }
        public async Task<GiftRespnseDto> AddGift(GiftCreateDto giftDto)
        {
            _logger.LogInformation("Post/ add gift called");
            var newGift = new Gift
            {
                Name = giftDto.Name,
                Description = giftDto.Description,
                //Price = giftDto.Price,
                ImageUrl = giftDto.ImageUrl,
                CategoryId = giftDto.CategoryId,
                DonorId = giftDto.DonorId
            };
            try
            {
                var createdGift = await _giftRepository.AddGift(newGift);
                _logger.LogInformation("Gift created successfully with Id {GiftId}", createdGift.Id);
                return MapToResponeseDto(createdGift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding gift Name={Name}", giftDto.Name);
                throw;
            }
        }
        public async Task<bool> DeleteGift(int id)
        {
            _logger.LogInformation("Delete/ delete gift called");
            try
            {
                return await _giftRepository.DeleteGift(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting gift with Id {GiftId}", id);
                throw;
            }
        }
        public async Task<GiftRespnseDto> UpdateGift(int giftId, GiftUpdateDto giftDto)
        {
            _logger.LogInformation("Put/ update gift called");
            try
            {
                var existingGift = await _giftRepository.GetById(giftId);
                if (existingGift == null)
                    return null;
                existingGift.Name = giftDto.Name;
                existingGift.Description = giftDto.Description;
                //existingGift.Price = giftDto.Price;
                existingGift.ImageUrl = giftDto.ImageUrl;
                existingGift.CategoryId = giftDto.CategoryId;
                existingGift.DonorId = giftDto.DonorId;
                //existingGift.Donor = giftDto.Donor;
                if(existingGift.WinnerId != null)
                    existingGift.WinnerId = giftDto.WinnerId;
                existingGift.IsDrawn = giftDto.IsDrawn;
                var updatedGift = await _giftRepository.UpdateGift(existingGift);
                _logger.LogInformation("Gift with Id {GiftId} updated successfully", giftId);
                return MapToResponeseDto(updatedGift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating gift with Id {GiftId}", giftId);
                throw;
            }
        }
        public async Task<IEnumerable<GiftRespnseDto>> FilterGifts(string? giftName,int? categoryId, string? donorName, int? buyersCount)
        {
            _logger.LogInformation("Get/ filter gifts called");
            try
            {
                var gifts = await _giftRepository.FilterGifts(giftName, categoryId, donorName, buyersCount);
                return gifts.Select(MapToResponeseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while filtering gifts");
                throw;
            }
        }
        public async Task<GiftRespnseDto> Lottery(int giftId)
        {
            _logger.LogInformation("Put/ lottery gift called");
            try
            {
                var gift = await _giftRepository.GetById(giftId);
                if (gift.IsDrawn)
                    throw new InvalidOperationException("This gift already drawn");
                var tickets = gift.Tickets.Where(t => !t.Purchase.IsDraft).ToList();
                if (!tickets.Any())
                    throw new InvalidOperationException("No tickets available for lottery");
                var random = new Random();
                var randomTicket = tickets[random.Next(tickets.Count)];
                gift.WinnerId = randomTicket.Purchase.BuyerId;
                gift.IsDrawn = true;
                var result = await _giftRepository.UpdateGift(gift);
                if (result == null)
                    throw new InvalidOperationException("error");
                _logger.LogInformation("Lottery completed successfully for gift Id {GiftId}", giftId);
                return MapToResponeseDto(gift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during lottery for gift Id {GiftId}", giftId);
                throw;
            }
        }
        public async Task<byte[]> GenerateWinnersReport()
        {
            _logger.LogInformation("Generating winners report with total revenue");

            // 1. הבאת הנתונים
            var gifts = await _giftRepository.GetAll(); //

            // שליפת כל הרכישות שאינן טיוטה וסיכום ה-TotalAmount שלהן
            // הערה: יש לוודא ש-PurchaseRepository מוזרק ב-Constructor
            var allPurchases = await _purchaseRepository.GetAll();
            decimal totalRevenue = allPurchases
                .Where(p => !p.IsDraft) // רק רכישות שאינן טיוטה
                .Sum(p => p.TotalAmount); // סיכום סך ההכנסות

            using (var memoryStream = new MemoryStream())
            {
                var encoding = Encoding.UTF8;
                await memoryStream.WriteAsync(encoding.GetPreamble(), 0, encoding.GetPreamble().Length);

                using (var sw = new StreamWriter(memoryStream, encoding, leaveOpen: true))
                {
                    // כתיבת כותרות הטבלה
                    await sw.WriteLineAsync("מזהה מתנה,שם מתנה,שם הזוכה,טלפון הזוכה");

                    // כתיבת שורות הזוכים
                    foreach (var gift in gifts)
                    {
                        if (gift.IsDrawn && gift.Winner != null) //
                        {
                            string line = $"{gift.Id},{gift.Name},{gift.Winner.FullName},{gift.Winner.PhoneNumber}";
                            await sw.WriteLineAsync(line);
                        }
                    }

                    // הוספת שורות רווח וסיכום הכנסות בתחתית
                    await sw.WriteLineAsync(); // שורה ריקה להפרדה
                    await sw.WriteLineAsync($",,סך כל ההכנסות:,{totalRevenue:N2} ₪");
                }

                return memoryStream.ToArray();
            }
        }
        private static GiftRespnseDto MapToResponeseDto(Gift gift)
        {
            return new GiftRespnseDto
            {
                Id = gift.Id,
                Name = gift.Name,
                Description = gift.Description,
                //Price = gift.Price,
                ImageUrl = gift.ImageUrl,
                CategoryId = gift.CategoryId,
                DonorId = gift.DonorId,
                Donor = gift.Donor,
                WinnerId = gift.WinnerId,
                IsDrawn = gift.IsDrawn
            };
        }
    }
}
