namespace server.Models
{
    public class Donor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
        public ICollection<Gift> Gifts { get; set; } = new List<Gift>();
    }
}
