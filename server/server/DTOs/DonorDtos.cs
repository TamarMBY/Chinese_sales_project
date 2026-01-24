using server.Models;
using System.ComponentModel.DataAnnotations;

namespace server.DTOs
{
    public class DonorRespnseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class DonorCreateDto
    {
        [Required, MaxLength(9)]
        public string Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, Phone]
        public string PhoneNumber { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }  
    }
    public class DonorUpdateDto
    {
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
    }
}
