using System.ComponentModel.DataAnnotations;

namespace server.DTOs
{
    public class PackageDtos
    {
        
    }
    public class PackageResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class PackageCreateDto
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty; 
        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Required, Range(10, double.MaxValue)]
        public decimal Price { get; set; }
    }
    public class PackageUpdateDto
    {
        [MaxLength(255)]
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        [Range(1, double.MaxValue)]
        public int Quantity { get; set; }
        [Range(10, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
