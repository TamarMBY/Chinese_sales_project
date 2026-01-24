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
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsDraft { get; set; }
    }
}
