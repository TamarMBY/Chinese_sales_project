using System.ComponentModel.DataAnnotations;

namespace server.DTOs
{
        public class GiftRespnseDto
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
        }

        public class GiftCreateDto
        {
            [Required, MaxLength(100)]
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            //public decimal Price { get; set; } = 10;
            public string? ImageUrl { get; set; }
            // Foreign Keys
            [Required]
            public int CategoryId { get; set; }
            [Required]
            public string DonorId { get; set; } = string.Empty;
        }
        public class GiftUpdateDto
        {
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            //public decimal Price { get; set; } = 10;
            public string? ImageUrl { get; set; }
            // Foreign Keys
            public int CategoryId { get; set; }
            public string DonorId { get; set; } = string.Empty;
            public string? WinnerId { get; set; }
            public bool IsDrawn { get; set; } = false;
        }
}
