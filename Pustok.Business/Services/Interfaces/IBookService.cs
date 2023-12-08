using Pustokk.Models;

namespace Pustokk.Services.Interfaces
{
    public interface IBookService
    {
        Task CreateAsync(Book entity);
        Task SoftDelete(int id);
        Task Delete(int id);
        Task<Book> GetByIdAsync(int id);
        Task<List<Book>> GetAllAsync();
		Task<List<Book>> GetAllRelatedBooksAsync(Book book);
		Task UpdateAsync(Book entity);
    }
}
