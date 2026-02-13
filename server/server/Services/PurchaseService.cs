using server.DTOs;
using server.Interfaces;
using server.Models;
using server.Repositories;
using StoreApi.Services;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace server.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configurtaion;
        private readonly ILogger<PurchaseService> _logger;
        private object PurchaseCreateDto;

        public PurchaseService(IConfiguration configuration, IPurchaseRepository purchaseRepository, ITicketService ticketService, ILogger<PurchaseService> logger, IUserService userService, IUserRepository userRepository)
        {
            _purchaseRepository = purchaseRepository;
            _ticketService = ticketService;
            _logger = logger;
            _userService = userService;
            _userRepository = userRepository;
            _configurtaion = configuration;

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

        public async Task<PurchaseRespnseDto> AddPackageToPurchase(int purchaseId, int packageId)
        {
            _logger.LogInformation("Post/ add package to purchase called");
            try
            {
                var purchase = await _purchaseRepository.GetById(purchaseId);
                if (purchase == null) return null;
                if (!purchase.IsDraft)
                    throw new InvalidOperationException("Cannot modify a finalized purchase.");
                var updated = await _purchaseRepository.AddPackageToPurchase(purchaseId, packageId);
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
                var deleted = await _purchaseRepository.DeletePackageFromPurchase(purchaseId, packageId);
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
                PurchasePackages = purchase.PurchasePackages,
                Tickets = purchase.Tickets
            };
        }

        private int GetRemainingTicketsCount(Purchase purchase)
        {
            _logger.LogInformation("GET / Get Remaining Tickets Count");
            try
            {

                int remainingTicketsCount = purchase.PurchasePackages?
                    .Where(pp => pp.Package != null)
                    .Sum(pp => pp.Package.Quantity * (pp.Quantity)) ?? 0;
                int useTicket = purchase.Tickets?.Count ?? 0;
                int remaining = remainingTicketsCount - useTicket;
                return remaining > 0 ? remaining : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Get Remaining Tickets Count");
                throw;
            }
        }

        public async Task<PurchaseRespnseDto> CompletionPurchase(int purchaseId, PurchaseUpdateDto purchase)
        {
            _logger.LogInformation("PUT complete purchase");
            try
            {
                var existPurchase = await _purchaseRepository.GetById(purchaseId);
                var sum = existPurchase.PurchasePackages?
                    .Where(pp => pp.Package != null)
                    .Sum(pp => pp.Package.Price * (pp.Quantity)) ?? 0;

                PurchaseRespnseDto updatePurchase = await UpdatePurchase(purchaseId, new PurchaseUpdateDto
                {
                    BuyerId = purchase.BuyerId,
                    IsDraft = false,
                    TotalAmount = sum
                });
                var user = await _userService.GetById(purchase.BuyerId);
                if (user == null)
                {
                    throw new Exception("user does not exist");
                }
                else
                {
                    try
                    {
                        sendEmail(updatePurchase, user, sum);
                        var NewPurchase = await _purchaseRepository.AddPurchase(new Purchase
                        {
                            BuyerId = purchase.BuyerId
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send email");
                    }
                }
                return updatePurchase; // או כל ערך אחר שאתה מחזיר
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while completing purchase");
                throw;
            }
        }
        private async void sendEmail( PurchaseRespnseDto purchase, UserResponseDto user, decimal sum)
        {
            try
            {
                // ודאי איות נכון של המשתנה
                // בניית גוף המייל בצורה בטוחה
                var mailText = $"שלום {user.FullName}, הרכישה שלך על סך {sum} שח הושלמה בהצלחה.";

                var emailData = new
                {
                    personalizations = new[]
                {
                            new { to = new[] { new { email = user.Email } } }
                        },
                    from = new { email = "tamar48719@gmail.com" },
                    subject = $"אישור רכישה - הזמנה {purchase.Id}",
                    content = new[]
                    {
                                new { type = "text/plain", value = $"שלום {user.FullName}, הרכישה שלך על סך {sum} שח הושלמה בהצלחה." }
                            }
                };

                // 2. המרה ל-JSON בעזרת הספרייה המובנית
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(emailData);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    var apiKey = _configurtaion["SendGrid:ApiKey"]; // ודאי איות נכון!

                    // שימי לב: "Bearer" בלי רווח בסוף!
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                    var response = await client.PostAsync("https://api.sendgrid.com/v3/mail/send", content);

                    // בדיקה מה קרה
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("SendGrid Status: {0}, Body: {1}", response.StatusCode, responseBody);
                }
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while completing purchase");
                throw;
            }
        }
        //public async void contactNewPurchase(string userId)
        //{
        //    var thisUser = await _userRepository.GetById(userId);
        //    var purchase = await _purchaseRepository.AddPurchase(new Purchase
        //    {
        //        BuyerId = userId
        //    });
        //    thisUser.Purchases.Add(purchase);            
        //}
    }
}
