namespace server.Models
{
    public class PurchasePackages
    {
        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        public int PackageId { get; set; }
        public Package Package { get; set; }
        public int Quantity { get; set; }
    }
}
