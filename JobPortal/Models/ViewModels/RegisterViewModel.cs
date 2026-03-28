using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password kam se kam 6 characters ka hona chahiye")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords match nahi kar rahe!")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role select karo")]
        public string Role { get; set; } // JobSeeker ya Employer

        // Employer ke liye
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        // JobSeeker ke liye
        [Display(Name = "Current City")]
        public string? CurrentCity { get; set; }
    }
}
