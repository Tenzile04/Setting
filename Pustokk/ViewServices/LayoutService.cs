using Pustokk.Business.Services.Interfaces;
using Pustokk.Models;
using Pustokk.Data.DAL;
using Pustokk.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Pustokk.ViewServices
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LayoutService(AppDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Setting>> GetBook()
        {
            var settings = _context.Settings.ToList();
            return settings;
        }

        public async Task<AppUser> GetAppUser()
        {
            AppUser user = null;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                string username = _httpContextAccessor.HttpContext.User.Identity.Name;
                user = await _userManager.FindByNameAsync(username);
            }
            
            return user;
        }

    }
}