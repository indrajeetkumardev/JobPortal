using JobPortal.Data;
using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    [Authorize(Roles = "JobSeeker")]  // ← Sirf JobSeeker access kar sakta hai
    public class JobSeekerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobSeekerController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);

            ViewBag.TotalApplications = await _context.JobApplications
                .CountAsync(a => a.ApplicantId == userId);
            ViewBag.Pending = await _context.JobApplications
                .CountAsync(a => a.ApplicantId == userId
                             && a.Status == "Pending");
            ViewBag.Shortlisted = await _context.JobApplications
                .CountAsync(a => a.ApplicantId == userId
                             && a.Status == "Shortlisted");
            ViewBag.Rejected = await _context.JobApplications
                .CountAsync(a => a.ApplicantId == userId
                             && a.Status == "Rejected");

            // Recent applications
            var applications = await _context.JobApplications
                .Include(a => a.Job)
                .Where(a => a.ApplicantId == userId)
                .OrderByDescending(a => a.AppliedDate)
                .Take(5)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            ViewBag.User = user;

            return View(applications);
        }

        // My Applications
        public async Task<IActionResult> MyApplications()
        {
            var userId = _userManager.GetUserId(User);
            var applications = await _context.JobApplications
                .Include(a => a.Job)
                .Where(a => a.ApplicantId == userId)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();
            return View(applications);
        }

        // GET: Apply
        [HttpGet]
        public async Task<IActionResult> Apply(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            // Already applied check
            var userId = _userManager.GetUserId(User);
            var alreadyApplied = await _context.JobApplications
                .AnyAsync(a => a.JobId == id && a.ApplicantId == userId);

            if (alreadyApplied)
            {
                TempData["Error"] = "Aap pehle se is job ke liye apply kar chuke ho!";
                return RedirectToAction("JobDetail", "Home", new { id });
            }

            ViewBag.Job = job;
            return View(new JobApplication { JobId = id });
        }

        // POST: Apply
        [HttpPost]
        public async Task<IActionResult> Apply(JobApplication application)
        {
            var userId = _userManager.GetUserId(User);

            // Naya object banao — purana form object mat use karo
            var newApplication = new JobApplication
            {
                JobId = application.JobId,
                ApplicantId = userId,
                CoverLetter = application.CoverLetter,
                ResumeUrl = application.ResumeUrl,
                Status = "Pending",
                AppliedDate = DateTime.Now
                // Id mat dena — SQL Server khud generate karega
            };

            _context.JobApplications.Add(newApplication);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Application successfully submit ho gayi!";
            return RedirectToAction("MyApplications");
        }
    }
}