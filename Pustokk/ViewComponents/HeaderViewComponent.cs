using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustokk.Core.Models;
using Pustokk.Data.DAL;
using Pustokk.Services.Interfaces;
using Pustokk.ViewModels;

namespace Pustokk.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IGenreService _genreService;
        private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public HeaderViewComponent(IGenreService genreService, AppDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _genreService = genreService;
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
		}

        public async Task<IViewComponentResult> InvokeAsync()
        {
			AppUser user = null;

			if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
			{
				user = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
			}

			HeaderViewModel headerViewModel = new HeaderViewModel();

            headerViewModel.Genres = await _genreService.GetAllAsync();
            headerViewModel.Settings = _context.Settings.ToList();
            headerViewModel.User = user;
            return View(headerViewModel);
        }
    }
}
