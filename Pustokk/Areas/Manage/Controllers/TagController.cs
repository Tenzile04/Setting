using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Pustokk.Business.Services.Interfaces;
using Pustokk.CustomExceptions.SliderException;
using Pustokk.Data.DAL;
using Pustokk.Models;
using Pustokk.Services.Implementations;
using Pustokk.Services.Interfaces;

namespace Pustokk.Areas.Manage.Controllers
{

    [Area("Manage")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ITagService _tagService;

        public TagController(AppDbContext context, ITagService tagService)
        {
            _context = context;
            _tagService = tagService;
        }
        public async Task<IActionResult> Index()
        {
            var existGenres = await _tagService.GetAllAsync();
            return View(existGenres);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid) return View(tag);

            _tagService.CreateAsync(tag);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int id)
        {
            Tag existTag = await _tagService.GetByIdAsync(id);
            if (existTag == null) return NotFound();
            return View(existTag);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Tag tag)
        {

            if (!ModelState.IsValid) return View();

            await _tagService.UpdateAsync(tag);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _tagService.Delete(id);
            }
            catch (InvalidNullReferanceException) { }

            return RedirectToAction("index");
        }
    }

}
