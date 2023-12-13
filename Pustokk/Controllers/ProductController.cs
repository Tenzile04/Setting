using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustokk.Core.Models;
using Pustokk.Data.DAL;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IBookRepository _bookRepository;

        public ProductController(IBookRepository bookRepository,IBookService bookService, UserManager<AppUser> userManager, AppDbContext context)
        {
            _bookRepository = bookRepository;
            _bookService = bookService;
            _userManager = userManager;
            _context = context;
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

        public async Task<IActionResult> AddToBasketAsync(int bookId)
        {

            if (!_bookRepository.Table.Any(x => x.Id == bookId)) return NotFound(); // 404

            List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();
            BasketItemViewModel basketItem = null;
            BasketItem userBasketItem = null;
            AppUser user = null;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            }

            if (user == null)
            {
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
            }
            else
            {
                userBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.BookId == bookId && x.AppUserId == user.Id);
                if (userBasketItem != null)
                {
                    userBasketItem.Count++;
                }
                else
                {
                    userBasketItem = new BasketItem
                    {
                        BookId = bookId,
                        Count = 1,
                        AppUserId = user.Id,
                        IsDeleted = false
                    };
                    _context.BasketItems.Add(userBasketItem);
                }
                await _context.SaveChangesAsync();
            }

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
            List<BasketItem> userBasketItems = new List<BasketItem>();
            CheckoutViewModel checkoutItem = null;
            AppUser user = null;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            }


            if (user == null)
            {
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
            }
            else
            {
                userBasketItems = await _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == user.Id).ToListAsync();

                foreach (var item in userBasketItems)
                {
                    checkoutItem = new CheckoutViewModel
                    {
                        Book = item.Book,
                        Count = item.Count
                    };
                    checkoutItemList.Add(checkoutItem);
                }
            }

            OrderViewModel orderViewModel = new OrderViewModel()
            {
                CheckoutViewModels = checkoutItemList,
                FullName = user?.FullName,
                Email = user?.Email,

            };
            return View(orderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderViewModel orderViewModel)
        {
            if (!ModelState.IsValid) return View();

            List<CheckoutViewModel> checkoutItemList = new List<CheckoutViewModel>();
            List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();
            List<BasketItem> userBasketItems = new List<BasketItem>();
            CheckoutViewModel checkoutItem = null;
            AppUser user = null;
            OrderItem orderItem = null;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            }

            Order order = new Order()
            {
                FullName = orderViewModel.FullName,
                Country = orderViewModel.Country,
                Address = orderViewModel.Address,
                Email = orderViewModel.Email,
                ZipCode = orderViewModel.ZipCode,
                Phone = orderViewModel.Phone,
                Note = orderViewModel.Note,
                UserId = user?.Id,
                OrderItems = new List<OrderItem>(),

            };


            if (user is null)
            {

                string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];

                if (basketItemListStr is not null)
                {
                    basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);

                    foreach (var item in basketItemList)
                    {
                        Book book = await _context.Books.FirstOrDefaultAsync(x => x.Id == item.BookId);

                        orderItem = new OrderItem()
                        {
                            Book = book,
                            BookName = book.Name,
                            CostPrice = book.CostPrice,
                            DiscountPercent = book.DiscountPercent,
                            SalePrice = book.SalePrice * ((100 - book.DiscountPercent) / 100),
                            Count = item.Count,
                            Order = order,
                        };

                        order.TotalPrice += orderItem.SalePrice * orderItem.Count;
                        order.OrderItems.Add(orderItem);
                    }

                }
            }
            else
            {
                userBasketItems = await _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == user.Id).ToListAsync();

                foreach (var item in userBasketItems)
                {
                    Book book = await _context.Books.FirstOrDefaultAsync(x => x.Id == item.BookId);

                    orderItem = new OrderItem()
                    {
                        Book = book,
                        BookName = book.Name,
                        CostPrice = book.CostPrice,
                        DiscountPercent = book.DiscountPercent,
                        SalePrice = book.SalePrice * ((100 - book.DiscountPercent) / 100),
                        Count = item.Count,
                        Order = order,
                    };

                    order.TotalPrice += orderItem.SalePrice * orderItem.Count;
                    order.OrderItems.Add(orderItem);
                }
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> SearchBooks(string value)
		{
			List<Book> searchedBooks = await _bookService.GetAllAsync(x => x.Name.ToLower().Contains(value.Trim().ToLower()));

			return Ok(searchedBooks);
		}
	}
}
 