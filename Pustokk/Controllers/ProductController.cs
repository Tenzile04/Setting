using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Pustokk.Models;
using Pustokk.Repositories.Interfaces;
using Pustokk.Services.Implementations;
using Pustokk.Services.Interfaces;
using Pustokk.ViewModels;

namespace Pustokk.Controllers
{
    public class ProductController : Controller
    {
		private readonly IBookService _bookService;
		private readonly IBookRepository _bookRepository;
		public ProductController(IBookRepository bookRepository, IBookService bookService)
        {
            _bookRepository = bookRepository;
			_bookService = bookService;
		}
        public IActionResult Index()
        {
            return View();
        }
		public async Task<IActionResult> Detail(int id)
		{
			Book book = await _bookService.GetByIdAsync(id);
			ProductDetailViewModel productDetailViewModel = new ProductDetailViewModel()
			{
				Book = book,
				RelatedBooks = await _bookService.GetAllRelatedBooksAsync(book)
			};

			return View(productDetailViewModel);
		}
        public async Task<IActionResult> GetBookModal(int id)
        {
            var book = await _bookService.GetByIdAsync(id);

            return PartialView("_BookModalPartial", book);
        }

        public IActionResult SetSession(string name)
        {
            HttpContext.Session.SetString("UserName", name);
            return Content("Added to session");
        }
        public IActionResult GetSession()
        {
            string username = HttpContext.Session.GetString("UserName");
            return Content(username);
        }
        public IActionResult RemoveSession()
        {
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("GetSession");
        }
        public IActionResult SetCookie(int id)
        {
            List<int> ids = new List<int>();

            string idStr = HttpContext.Request.Cookies["UserId"];

            if (idStr != null)
            {
                ids = JsonConvert.DeserializeObject<List<int>>(idStr);
            }

            ids.Add(id);

            idStr = JsonConvert.SerializeObject(ids);

            HttpContext.Response.Cookies.Append("userid", idStr);

            return Content("Added to cookie");
        }
        public IActionResult GetCookies()
        {
            List<int> ids = new List<int>();

            string idstr = HttpContext.Request.Cookies["Userid"];

            ids = JsonConvert.DeserializeObject<List<int>>(idstr);

            return Json(idstr);
        }

        public IActionResult AddToBasket(int bookId)
        {

            if (!_bookRepository.Table.Any(x => x.Id == bookId)) return NotFound(); // 404

            List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();
            BasketItemViewModel basketItem = null;
            string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];

            if (basketItemListStr != null)
            {
                basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);

                basketItem = basketItemList.FirstOrDefault(x => x.BookId == bookId);

                if (basketItem != null)
                {
                    basketItem.Count++;
                }
                else
                {
                    basketItem = new BasketItemViewModel()
                    {
                        BookId = bookId,
                        Count = 1
                    };

                    basketItemList.Add(basketItem);
                }
            }
            else
            {
                basketItem = new BasketItemViewModel()
                {
                    BookId = bookId,
                    Count = 1
                };

                basketItemList.Add(basketItem);
            }

            basketItemListStr = JsonConvert.SerializeObject(basketItemList);

            HttpContext.Response.Cookies.Append("BasketItems", basketItemListStr);

            return Ok(); //200
        }

        public IActionResult GetBasketItems()
        {
            List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();

            string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];

            if (basketItemListStr != null)
            {
                basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);
            }

            return Json(basketItemList);
        }

        public async Task<IActionResult> Checkout()
        {
            List<CheckoutViewModel> checkoutItemList = new List<CheckoutViewModel>();
            List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();
            CheckoutViewModel checkoutItem = null;

            string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];
            if (basketItemListStr != null)
            {
                basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);

                foreach (var item in basketItemList)
                {
                    checkoutItem = new CheckoutViewModel
                    {
                        Book = await _bookRepository.GetByIdAsync(x => x.Id == item.BookId),
                        Count = item.Count
                    };
                    checkoutItemList.Add(checkoutItem);
                }
            }

            return View(checkoutItemList);
        }

    }
}
 