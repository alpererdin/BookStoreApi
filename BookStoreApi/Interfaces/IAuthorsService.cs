using BookStoreApi.Models;
using System.Threading.Tasks;

namespace BookStoreApi.Interfaces;

public interface IAuthorsService : IMongoDbService<Author>
{
    Task<Author?> GetByNameAsync(string name);
}