using Pustokk.Data.DAL;
using Pustokk.Models;
using Pustokk.Repositories.Interfaces;

namespace Pustokk.Repositories.Implementations
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(AppDbContext context) : base(context)
        {
        }
    }
}
