using Microsoft.AspNetCore.Builder;
using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Job title zaroori hai")]
        [MaxLength(100)]
        [Display(Name = "Job Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Company name zaroori hai")]
        [MaxLength(100)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Location zaroori hai")]
        [MaxLength(100)]
        public string Location { get; set; }

        [Required(ErrorMessage = "Description zaroori hai")]
        [Display(Name = "Job Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Requirements zaroori hain")]
        public string Requirements { get; set; }

        [Display(Name = "Salary Range")]
        public string? SalaryRange { get; set; }

        [Required]
        [Display(Name = "Job Type")]
        public string JobType { get; set; } // Full-time, Part-time, Remote

        [Required]
        [Display(Name = "Experience Required")]
        public string ExperienceRequired { get; set; }

        [Display(Name = "Last Date to Apply")]
        [DataType(DataType.Date)]
        public DateTime LastDate { get; set; }

        public DateTime PostedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Employer ka ID jo job post kiya
        public string EmployerId { get; set; }
        public ApplicationUser? Employer { get; set; }

        // Applications
        public List<JobApplication> Applications { get; set; } = new();
    }
}
