using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustokk.Areas.ViewModels;
using Pustokk.Core.Models;

namespace Pustokk.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAsync(AdminLoginViewModel adminLoginVM)
        {
            if (!ModelState.IsValid) return View(adminLoginVM);
            AppUser admin = null;
            admin = await _userManager.FindByNameAsync(adminLoginVM.Username);
            if (admin is null)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(admin, adminLoginVM.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View();
            }
            return RedirectToAction("Index", "Dashboard");
        }
    }

}
