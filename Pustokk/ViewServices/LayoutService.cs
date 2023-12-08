using Pustokk.Business.Services.Interfaces;
using Pustokk.Models;
using Pustokk.Data.DAL;
using Pustokk.Core.Models;

namespace Pustok.ViewServices
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        public LayoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Setting>> GetBook()
        {
            var settings = _context.Settings.ToList();
            return settings;
        }

    }
}