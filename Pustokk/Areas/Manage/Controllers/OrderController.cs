using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustokk.Core.Models;
using Pustokk.Data.DAL;
using Pustokk.PaginationHelper;

namespace Pustokk.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page=1)
        {
            var query = _context.Orders.AsQueryable();
            //List<Order> orders = await _context.Orders.ToListAsync();
            PaginatedList<Order> paginatedOrder =  PaginatedList<Order>.Create(query, page, 1);
            return View(paginatedOrder);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Order order = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            if (order is null) return NotFound();

            return View(order);
        }

        public async Task<IActionResult> Accept(int id)
        {
            Order order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order is null) return NotFound();
            order.OrderStatus = Core.Enums.OrderStatus.Accepted;

            await _context.SaveChangesAsync();

            return RedirectToAction("index", "order");
        }

        public async Task<IActionResult> Reject(int id, string AdminComment)
        {

            Order order = await _context.Orders.Include(x=>x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            if (order is null) return NotFound();
            if (AdminComment == null)
            {
                ModelState.AddModelError("AdminComment", "Must be written");
                return View("detail",order);
            }

            order.OrderStatus = Core.Enums.OrderStatus.Rejected;
            order.AdminComment = AdminComment;

            await _context.SaveChangesAsync();


            return RedirectToAction("index", "order");
        }
    }
}
