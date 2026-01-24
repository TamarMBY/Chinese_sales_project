namespace server.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public decimal TotalAmount { get; set; } = 0;
        public DateTime OrderDate { get; set; }
        public bool IsDraft { get; set; } = false;
        public User Buyer { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<Package> Packages { get; set; } = new List<Package>();
    }
}
