using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Repositories;

namespace server.Services
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _giftRepository;

        public GiftService(IGiftRepository giftRepository)
        {
            _giftRepository = giftRepository;
        }
        public async Task<IEnumerable<GiftRespnseDto>> GetAll()
        {
            var gifts = await _giftRepository.GetAll();
            return gifts.Select(MapToResponeseDto);
        }
        public async Task<GiftRespnseDto> GetById(int id)
        {
            var gift = await _giftRepository.GetById(id);
            return gift != null ? MapToResponeseDto(gift) : null;
        }

        public async Task<GiftRespnseDto> AddGift(GiftCreateDto giftDto)
        {
            var newGift = new Gift
            {
                Name = giftDto.Name,
                Description = giftDto.Description,
                Price = giftDto.Price,
                ImageUrl = giftDto.ImageUrl,
                CategoryId = giftDto.CategoryId,
                DonorId = giftDto.DonorId
            };

            var createdGift = await _giftRepository.AddGift(newGift);
            return MapToResponeseDto(createdGift);
        }

        public async Task<bool> DeleteGift(int id)
        {
            return await _giftRepository.DeleteGift(id);
        }

        public async Task<GiftRespnseDto> UpdateGift(int giftId,GiftUpdateDto giftDto)
        {
            var existingGift = await _giftRepository.GetById(giftId);
            if (existingGift == null)
                return null;
            existingGift.Name = giftDto.Name;
            existingGift.Description = giftDto.Description;
            existingGift.Price = giftDto.Price;
            existingGift.ImageUrl = giftDto.ImageUrl;
            existingGift.CategoryId = giftDto.CategoryId;
            existingGift.DonorId = giftDto.DonorId;
            existingGift.WinnerId = giftDto.WinnerId;
            existingGift.IsDrawn = giftDto.IsDrawn;
            var updatedGift = await _giftRepository.UpdateGift(existingGift);
            return MapToResponeseDto(updatedGift);
        }
        public async Task<GiftRespnseDto> Lottery(int giftId)
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
            return MapToResponeseDto(gift);
        }
        public async Task<IEnumerable<GiftRespnseDto>> FilterGifts(string? giftName, string? donorName, int? buyersCount)
        {
            var gifts = await _giftRepository.FilterGifts(giftName, donorName, buyersCount);
            return gifts.Select(MapToResponeseDto);
        }
        private static GiftRespnseDto MapToResponeseDto(Gift gift)
        {
            return new GiftRespnseDto
            {
                Id = gift.Id,
                Name = gift.Name,
                Description = gift.Description,
                Price = gift.Price,
                ImageUrl = gift.ImageUrl,
                CategoryId = gift.CategoryId,
                DonorId = gift.DonorId,
                WinnerId = gift.WinnerId,
                IsDrawn = gift.IsDrawn
            };
        }

       
    }
}
