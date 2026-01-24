using System.ComponentModel.DataAnnotations;

namespace server.DTOs
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class UserCreateDto
    {
        [Required, MaxLength(100)]
        public string Id { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; }
        [Required, MinLength(5)]
        public string UserName { get; set; }
        [Required, MinLength(8)]
        public string Password { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }

    public class UserUpdateDto
    {
        [MinLength(5)]
        public string UserName { get; set; }
        [MaxLength(100)]
        public string FullName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
