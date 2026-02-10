using server.Models;
using System.ComponentModel.DataAnnotations;

namespace server.DTOs
{
    public class PurchaseRespnseDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsDraft { get; set; }
        public ICollection<PurchasePackages> PurchasePackages { get; set; } = new List<PurchasePackages>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }

    public class PurchaseCreateDto
    {
        [Required, MaxLength(9)]
        public string BuyerId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }
    public class PurchaseUpdateDto
    {
        [Required, MaxLength(9)]
        public string BuyerId { get; set; }
        public decimal  TotalAmount { get; set; }
        public DateTime  OrderDate { get; set; }
        public bool  IsDraft { get; set; }
    }
}
