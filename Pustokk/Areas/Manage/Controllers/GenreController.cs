using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustokk.CustomExceptions.SliderException;
using Pustokk.Data.DAL;
using Pustokk.Models;
using Pustokk.Services.Implementations;
using Pustokk.Services.Interfaces;

namespace Pustokk.Areas.Manage.Controllers
{
    [Area("manage")]
    public class GenreController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IGenreService _genreService;

        public GenreController(AppDbContext context, IGenreService genreService)
        {
            _context = context;
            this._genreService = genreService;
        }
        public async Task<IActionResult> Index()
        {
            var existGenres = await _genreService.GetAllAsync();
            return View(existGenres);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            if (!ModelState.IsValid) return View(genre);
           
            _genreService.CreateAsync(genre);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int id)
        {
            Genre existGenres = await _genreService.GetByIdAsync(id);
            if (existGenres == null) return NotFound();
            return View(existGenres);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Genre genre)
        {
        
            if (!ModelState.IsValid) return View();

            await _genreService.UpdateAsync(genre);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _genreService.Delete(id);
            }
            catch (InvalidNullReferanceException) { }

            return RedirectToAction("index");
        }
    }
}
