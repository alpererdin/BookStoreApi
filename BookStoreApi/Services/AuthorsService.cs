using BookStoreApi.Models;
using Microsoft.Extensions.Options;

namespace BookStoreApi.Services;

public class AuthorsService : MongoDbService<Author>
{
    public AuthorsService(IOptions<BookStoreDatabaseSettings> options) : base(options)
    {
    }
}
