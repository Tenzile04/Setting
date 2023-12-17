using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustokk.Core.Models;

namespace Pustokk.Controllers
{
	public class ChatController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
        public ChatController(UserManager<AppUser> userManager)
        {
			_userManager = userManager;
        }
        public async Task<IActionResult> Index()
		{
			var users =await _userManager.Users.ToListAsync();
			return View(users);
		}
	}
}
