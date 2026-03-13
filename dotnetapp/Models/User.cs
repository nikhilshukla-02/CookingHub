using System.ComponentModel.DataAnnotations;

namespace dotnetapp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 255 characters")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must have at least one uppercase, one lowercase, one digit, and one special character (@$!%*?&)")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_\s]+$", ErrorMessage = "Username can only contain letters, digits, underscores, and spaces")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "Mobile number must be between 7 and 15 digits")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Mobile number must contain only digits and an optional leading +")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "User role is required")]
        [RegularExpression(@"^(Admin|User)$", ErrorMessage = "UserRole must be either 'Admin' or 'User'")]
        public string UserRole { get; set; } = "User";

        // NEW FIELDS FOR EMAIL VERIFICATION
        public bool IsVerified { get; set; } = false;
        public string? VerificationToken { get; set; }
    }
}
