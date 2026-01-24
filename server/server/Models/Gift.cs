namespace server.Models
{
    public class Gift
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        //public decimal Price { get; set; } = 10;
        public string? ImageUrl { get; set; }
        // Foreign Keys
        public int CategoryId { get; set; }
        public string DonorId { get; set; } = string.Empty;
        public string? WinnerId { get; set; }
        public bool IsDrawn { get; set; } = false;
        public virtual Category Category { get; set; } = null!;
        public virtual Donor Donor { get; set; } = null!;
        public virtual User? Winner { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}