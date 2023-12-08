using Microsoft.AspNetCore.Mvc;
using Pustokk.CustomExceptions.SliderException;
using Pustokk.Data.DAL;
using Pustokk.Models;
using Pustokk.Services.Implementations;
using Pustokk.Services.Interfaces;

namespace Pustokk.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAuthorService _authorService;

        public AuthorController(AppDbContext context, IAuthorService authorService)
        {
            _context = context;
            _authorService = authorService;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var existAuthor = await _authorService.GetAllAsync();
            return View(existAuthor);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(Author author)
        {
            if (!ModelState.IsValid) return View();

            await _authorService.CreateAsync(author);
           
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> UpdateAsync(int id)
        {
            var existAuthor = await _authorService.GetByIdAsync(id);
            if (existAuthor == null) return NotFound();
            return View(existAuthor);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(Author author)
        {
            Author existAuthor = await _authorService.GetByIdAsync(author.Id);
            if (existAuthor == null) return NotFound();
            if (!ModelState.IsValid) return View(existAuthor);

            await _authorService.UpdateAsync(author);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _authorService.Delete(id);
            }
            catch (InvalidNullReferanceException) { }

            return RedirectToAction("index");
        }
    }
}
