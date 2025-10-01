using BookStoreApi.Interfaces;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BookStoreApi.Services;

public class AuthorsService : MongoDbService<Author>, IAuthorsService
{
    public AuthorsService(IMongoDatabase database) : base(database)
    {
    }


    public virtual async Task<Author?> GetByNameAsync(string name) =>
        await _collection.Find(x => x.AuthorName == name).FirstOrDefaultAsync();
}