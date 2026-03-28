using JobPortal.Models;
using JobPortal.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    CompanyName = model.CompanyName,
                    CurrentCity = model.CurrentCity,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Role assign karo
                    await _userManager.AddToRoleAsync(user, model.Role);

                    // Login karwa do
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Success"] = "Account successfully bana! Welcome!";

                    // Role ke hisaab se redirect karo
                    return model.Role switch
                    {
                        "Employer" => RedirectToAction("Dashboard", "Employer"),
                        "JobSeeker" => RedirectToAction("Dashboard", "JobSeeker"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }

                // Errors dikhao
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    TempData["Success"] = $"Welcome back, {user.FullName}!";

                    return roles[0] switch
                    {
                        "Admin" => RedirectToAction("Dashboard", "Admin"),
                        "Employer" => RedirectToAction("Dashboard", "Employer"),
                        "JobSeeker" => RedirectToAction("Dashboard", "JobSeeker"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }

                ModelState.AddModelError("", "Email ya password galat hai!");
            }
            return View(model);
        }

        // Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();
    }
}