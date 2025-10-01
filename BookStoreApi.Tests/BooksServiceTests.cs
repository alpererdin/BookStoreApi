using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using BookStoreApi.Services;
using BookStoreApi.Models;

namespace BookStoreApi.Tests;

public class BooksServiceTests
{
    [Fact]
    public void BooksService_CanBeConstructed()
    {
         
        var mockOptions = new Mock<IOptions<BookStoreDatabaseSettings>>();
        var settings = new BookStoreDatabaseSettings
        {
            ConnectionString = "mongodb://dummyserver:27017",
            DatabaseName = "TestDb",
            BooksCollectionName = "Books",
            AuthorsCollectionName = "Authors"
        };
        
        mockOptions.Setup(o => o.Value).Returns(settings);

 
        var service = new BooksService(mockOptions.Object);

        
        Assert.NotNull(service);
    }
}