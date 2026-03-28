using Microsoft.AspNetCore.Identity;

namespace JobPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Job Seeker ke liye
        public string? ResumeUrl { get; set; }
        public string? Skills { get; set; }
        public string? CurrentCity { get; set; }

        // Employer ke liye
        public string? CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
    }
}
