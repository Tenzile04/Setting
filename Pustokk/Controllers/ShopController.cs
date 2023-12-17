using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustokk.Data.DAL;
using Pustokk.ViewModels;

namespace Pustokk.Controllers
{
	public class ShopController : Controller
	{
		private readonly AppDbContext _context;
		public ShopController(AppDbContext context)
		{
			_context = context;	
		}
		public async Task<IActionResult> Index(int? genreId)
		{
			var query = _context.Books.Include(x => x.Author).Include(x => x.BookImages).AsQueryable();
			if(genreId != null)
			{
				query=query.Where(x=>x.GenreId==genreId);
			}
			
			ShopViewModel model = new ShopViewModel()
			{
				Books = await query.ToListAsync(),
				Genres = await _context.Genres.Include(x=>x.Books).ToListAsync()
			};



			return View(model);
		}
	}
}
