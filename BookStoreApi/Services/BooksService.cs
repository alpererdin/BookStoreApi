using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;  
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Services;

public class BooksService : MongoDbService<Book>
{
    public BooksService(IOptions<BookStoreDatabaseSettings> options) : base(options)
    {
    }

    public async Task<Book?> GetByNameAndAuthorAsync(string bookName, string authorId) =>
        await _collection.Find(x => x.BookName == bookName && x.AuthorId == authorId).FirstOrDefaultAsync();
}