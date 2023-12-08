using Pustokk.Models;
using Pustokk.Repositories.Interfaces;
using Pustokk.Services.Interfaces;

namespace Pustokk.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }
        public async Task CreateAsync(Author entity)
        {
            await _authorRepository.CreateAsync(entity);
            await _authorRepository.CommitAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _authorRepository.GetByIdAsync(x => x.Id == id && x.IsDeleted == false);
            if (entity is null) throw new NullReferenceException();

            _authorRepository.Delete(entity);
            await _authorRepository.CommitAsync();
        }

        public async Task<List<Author>> GetAllAsync()
        {
            return await _authorRepository.GetAllAsync();
        }

        public async Task<Author> GetByIdAsync(int id)
        {
            var entity = await _authorRepository.GetByIdAsync(x => x.Id == id && x.IsDeleted == false);
            if (entity is null) throw new NullReferenceException();
            return entity;
        }

        public async Task UpdateAsync(Author author)
        {
            var existEntity = await _authorRepository.GetByIdAsync(x => x.Id == author.Id && x.IsDeleted == false);

            if (_authorRepository.Table.Any(x => x.FullName == author.FullName && existEntity.Id != author.Id))
                throw new NullReferenceException();

            existEntity.FullName = author.FullName;
            await _authorRepository.CommitAsync();
        }
    }
}
