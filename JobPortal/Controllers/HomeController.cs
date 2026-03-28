using JobPortal.Data;
using JobPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalJobs = await _context.Jobs
                .CountAsync(j => j.IsActive);
            ViewBag.TotalEmployers = await _context.Users
                .CountAsync();
            ViewBag.TotalSeekers = await _context.Users
                .CountAsync();
            ViewBag.TotalApplications = await _context.JobApplications
                .CountAsync();
            ViewBag.LatestJobs = await _context.Jobs
                .Where(j => j.IsActive)
                .OrderByDescending(j => j.PostedDate)
                .Take(6)
                .ToListAsync();
            return View();
        }

        // Browse All Jobs
        public async Task<IActionResult> Jobs(string search, string location)
        {
            ViewBag.Search = search;
            ViewBag.Location = location;

            var query = _context.Jobs
                .Where(j => j.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(j =>
                    j.Title.Contains(search) ||
                    j.CompanyName.Contains(search) ||
                    j.Description.Contains(search));

            if (!string.IsNullOrEmpty(location))
                query = query.Where(j => j.Location.Contains(location));

            var jobs = await query
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            return View(jobs);
        }

        // Job Detail
        public async Task<IActionResult> JobDetail(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();
            return View(job);
        }
    }
}