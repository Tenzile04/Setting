using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustokk.Core.Models;
using Pustokk.ViewModels;

namespace Pustokk.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager,RoleManager<IdentityRole>roleManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(MemberRegisterViewModel memberRegisterVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = null;

            user = await _userManager.FindByNameAsync(memberRegisterVM.Username);
            if (user is not null)
            {
                ModelState.AddModelError("Username", "Username already exist!");
                return View();
            }
            user = await _userManager.FindByEmailAsync(memberRegisterVM.Email);

            if (user is not null)
            {
                ModelState.AddModelError("Email", "Email already exist!");
                return View();
            }
            AppUser appUser = new AppUser
            {
                FullName = memberRegisterVM.Fullname,
                UserName = memberRegisterVM.Username,
                Email = memberRegisterVM.Email,
                BirthDate = memberRegisterVM.Birthdate
            };
            var result = await _userManager.CreateAsync(appUser, memberRegisterVM.Password);
           
            if (result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View();
                }
            }
            await _userManager.AddToRoleAsync(appUser, "Member");
            await _signInManager.SignInAsync(appUser, isPersistent: false);

            return RedirectToAction("index", "Home");
        }
		public async Task<IActionResult> Login(MemberLoginViewModel memberLoginVM)
		{
			if (!ModelState.IsValid) return View();
			AppUser user = null;

			user = await _userManager.FindByNameAsync(memberLoginVM.Username);

			if (user == null)
			{
				ModelState.AddModelError("", "Invalid Username or Password or Email!");
				return View();
			}

			var result = await _signInManager.PasswordSignInAsync(user, memberLoginVM.Password, false, false);

			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Invalid Username or Password or Email!");
				return View();
			}

			user = await _userManager.FindByEmailAsync(memberLoginVM.Email);

			if (user == null)
			{
				ModelState.AddModelError("", "Invalid Username or Password! or Email!");
				return View();
			}
			return RedirectToAction("Index", "Home");
		}
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction("Login", "Account");
		}
	}
}
