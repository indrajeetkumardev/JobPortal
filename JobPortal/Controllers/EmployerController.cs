using JobPortal.Data;
using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployerController(
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

            ViewBag.TotalJobs = await _context.Jobs
                .CountAsync(j => j.EmployerId == userId);
            ViewBag.ActiveJobs = await _context.Jobs
                .CountAsync(j => j.EmployerId == userId && j.IsActive);
            ViewBag.TotalApplications = await _context.JobApplications
                .Include(a => a.Job)
                .CountAsync(a => a.Job.EmployerId == userId);

            var recentJobs = await _context.Jobs
                .Where(j => j.EmployerId == userId)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedDate)
                .Take(5)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            ViewBag.User = user;

            return View(recentJobs);
        }

        // My Jobs
        public async Task<IActionResult> MyJobs()
        {
            var userId = _userManager.GetUserId(User);
            var jobs = await _context.Jobs
                .Where(j => j.EmployerId == userId)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
            return View(jobs);
        }

        // GET: Post Job
        [HttpGet]
        public IActionResult PostJob() => View(new Job());

        // POST: Post Job
        // POST: Post Job
        [HttpPost]
        public async Task<IActionResult> PostJob(Job job)
        {
            // EmployerId pehle set karo
            job.EmployerId = _userManager.GetUserId(User);

            // Phir ModelState se EmployerId ki error hatao
            ModelState.Remove("EmployerId");
            ModelState.Remove("Employer");

            if (ModelState.IsValid)
            {
                job.PostedDate = DateTime.Now;
                job.IsActive = true;
                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"'{job.Title}' job successfully post ho gayi!";
                return RedirectToAction("MyJobs");
            }

            // Debug — kaunsi fields fail ho rahi hain
            foreach (var error in ModelState)
            {
                if (error.Value.Errors.Any())
                {
                    Console.WriteLine($"Field: {error.Key} — " +
                        string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
                }
            }

            return View(job);
        }

        // GET: Edit Job
        [HttpGet]
        public async Task<IActionResult> EditJob(int id)
        {
            var userId = _userManager.GetUserId(User);
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == userId);
            if (job == null) return NotFound();
            return View(job);
        }

        // POST: Edit Job
        [HttpPost]
        public async Task<IActionResult> EditJob(Job job)
        {
            if (ModelState.IsValid)
            {
                job.EmployerId = _userManager.GetUserId(User);
                _context.Jobs.Update(job);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job update ho gayi!";
                return RedirectToAction("MyJobs");
            }
            return View(job);
        }

        // Delete Job
        public async Task<IActionResult> DeleteJob(int id)
        {
            var userId = _userManager.GetUserId(User);
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == userId);
            if (job != null)
            {
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Job delete ho gayi!";
            }
            return RedirectToAction("MyJobs");
        }

        // View Applicants
        public async Task<IActionResult> Applicants(int jobId)
        {
            var userId = _userManager.GetUserId(User);
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == userId);
            if (job == null) return NotFound();

            var applications = await _context.JobApplications
                .Include(a => a.Applicant)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();

            ViewBag.Job = job;
            return View(applications);
        }

        // Update Application Status
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int appId, string status)
        {
            var app = await _context.JobApplications.FindAsync(appId);
            if (app != null)
            {
                app.Status = status;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Status '{status}' update ho gaya!";
            }
            return RedirectToAction("Applicants", new { jobId = app.JobId });
        }
    }
}