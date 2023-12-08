using Microsoft.AspNetCore.Mvc;
using Pustokk.Data.DAL;
using Pustokk.Services.Interfaces;
using Pustokk.ViewModels;

namespace Pustokk.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IGenreService _genreService;
        private readonly AppDbContext _context;
        public HeaderViewComponent(IGenreService genreService, AppDbContext context)
        {
            _context = context;
            _genreService = genreService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            HeaderViewModel headerViewModel = new HeaderViewModel();

            headerViewModel.Genres = await _genreService.GetAllAsync();
            headerViewModel.Settings = _context.Settings.ToList();
            return View(headerViewModel);
        }
    }
}
