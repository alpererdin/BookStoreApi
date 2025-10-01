using BookStoreApi.Models;
using System.Threading.Tasks;

namespace BookStoreApi.Interfaces;

public interface IBooksService : IMongoDbService<Book>
{
    Task<Book?> GetByNameAndAuthorAsync(string bookName, string authorId);
}