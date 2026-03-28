using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class JobApplication
    {
        public int Id { get; set; }

        public int JobId { get; set; }
        public Job? Job { get; set; }

        public string ApplicantId { get; set; }
        public ApplicationUser? Applicant { get; set; }

        [Display(Name = "Cover Letter")]
        public string? CoverLetter { get; set; }

        public string? ResumeUrl { get; set; }

        public string Status { get; set; } = "Pending";
        // Pending, Reviewed, Shortlisted, Rejected

        public DateTime AppliedDate { get; set; } = DateTime.Now;
    }
}
