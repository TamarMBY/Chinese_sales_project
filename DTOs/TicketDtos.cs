using System.ComponentModel.DataAnnotations;

namespace server.DTOs
{
    public class TicketRespnseDto
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public int GiftId { get; set; }
        public int Quantity { get; set; }
    }

    public class TicketCreateDto
    {
        [Required]
        public int PurchaseId { get; set; }
        [Required]
        public int GiftId { get; set; }
    }
    public class TicketUpdateDto
    {
        public int PurchaseId { get; set; }
        public int GiftId { get; set; }
        public int Quantity { get; set; }
    }
}
