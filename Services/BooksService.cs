using BookStoreApi.Models;
using Microsoft.Extensions.Options;

namespace BookStoreApi.Services;

public class BooksService : MongoDbService<Book>
{
    public BooksService(IOptions<BookStoreDatabaseSettings> options) : base(options)
    {
    }

     
}