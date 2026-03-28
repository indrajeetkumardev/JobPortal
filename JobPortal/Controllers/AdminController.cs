using JobPortal.Data;
using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalJobs = await _context.Jobs.CountAsync();
            ViewBag.ActiveJobs = await _context.Jobs
                .CountAsync(j => j.IsActive);
            ViewBag.TotalApplications = await _context.JobApplications
                .CountAsync();

            // Recent jobs
            var recentJobs = await _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedDate)
                .Take(8)
                .ToListAsync();

            return View(recentJobs);
        }

        // All Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            // Har user ka role bhi lao
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "N/A";
            }
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        // All Jobs
        public async Task<IActionResult> Jobs()
        {
            var jobs = await _context.Jobs
                .Include(j => j.Employer)
                .Include(j => j.Applications)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
            return View(jobs);
        }

        // Toggle Job Active/Inactive
        public async Task<IActionResult> ToggleJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                job.IsActive = !job.IsActive;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Job '{job.Title}' " +
                    (job.IsActive ? "activate" : "deactivate") + " ho gayi!";
            }
            return RedirectToAction("Jobs");
        }

        // Delete User
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                TempData["Success"] = "User delete ho gaya!";
            }
            return RedirectToAction("Users");
        }
    }
}