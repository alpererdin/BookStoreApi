using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Controllers;
using BookStoreApi.Models;
using BookStoreApi.Models.DTOs.Requests;
using BookStoreApi.Models.DTOs.Responses;
using BookStoreApi.Interfaces;
using System.Threading.Tasks;

namespace BookStoreApi.Tests;

public class BooksControllerTests
{
    private readonly Mock<IBooksService> _booksServiceMock;
    private readonly Mock<IAuthorsService> _authorsServiceMock;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _booksServiceMock = new Mock<IBooksService>();
        _authorsServiceMock = new Mock<IAuthorsService>();
        _controller = new BooksController(_booksServiceMock.Object, _authorsServiceMock.Object);
    }

    [Fact]
    public async Task Post_WithNewAuthorName_CreatesAuthorAndBook()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            BookName = "Test Book",
            Price = 50m,
            Category = "Fiction",
            NewAuthorName = "New Author"
        };

        _authorsServiceMock.Setup(x => x.GetByNameAsync("New Author"))
            .ReturnsAsync((Author?)null);
        _authorsServiceMock.Setup(x => x.CreateAsync(It.IsAny<Author>()))
            .Callback<Author>(a => a.Id = "author1")
            .Returns(Task.CompletedTask);
        _booksServiceMock.Setup(x => x.GetByNameAndAuthorAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Book?)null);
        _booksServiceMock.Setup(x => x.CreateAsync(It.IsAny<Book>()))
            .Callback<Book>(b => b.Id = "book1")
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Post(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<BookResponse>>>(result);
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var apiResponse = Assert.IsType<ApiResponse<BookResponse>>(createdResult.Value);

        Assert.True(apiResponse.Success);
        Assert.Equal("Book created successfully", apiResponse.Message);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("Test Book", apiResponse.Data.BookName);
        Assert.Equal(50m, apiResponse.Data.Price);
        Assert.Equal("Fiction", apiResponse.Data.Category);
        Assert.Equal("New Author", apiResponse.Data.AuthorName);

        _authorsServiceMock.Verify(x => x.CreateAsync(It.IsAny<Author>()), Times.Once);
        _booksServiceMock.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task Post_WithExistingAuthorName_CreatesOnlyBook()
    {
        // Arrange
        var existingAuthor = new Author
        {
            Id = "author123",
            AuthorName = "Existing Author"
        };

        var request = new CreateBookRequest
        {
            BookName = "New Book",
            Price = 40m,
            Category = "Science",
            NewAuthorName = "Existing Author"
        };

        _authorsServiceMock.Setup(x => x.GetByNameAsync("Existing Author"))
            .ReturnsAsync(existingAuthor);
        _booksServiceMock.Setup(x => x.GetByNameAndAuthorAsync(It.IsAny<string>(), "author123"))
            .ReturnsAsync((Book?)null);
        _booksServiceMock.Setup(x => x.CreateAsync(It.IsAny<Book>()))
            .Callback<Book>(b => b.Id = "book1")
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Post(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<BookResponse>>>(result);
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var apiResponse = Assert.IsType<ApiResponse<BookResponse>>(createdResult.Value);

        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("author123", apiResponse.Data.AuthorId);
        Assert.Equal("Existing Author", apiResponse.Data.AuthorName);

        _authorsServiceMock.Verify(x => x.CreateAsync(It.IsAny<Author>()), Times.Never);
        _booksServiceMock.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task Post_WithInvalidAuthorId_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            BookName = "Test Book",
            Price = 30m,
            Category = "Horror",
            AuthorId = "invalid-id"
        };

        _authorsServiceMock.Setup(x => x.GetAsync("invalid-id"))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await _controller.Post(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<BookResponse>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var apiResponse = Assert.IsType<ApiResponse<BookResponse>>(badRequestResult.Value);

        Assert.False(apiResponse.Success);
        Assert.Equal("The provided AuthorId does not exist.", apiResponse.Message);
    }

    [Fact]
    public async Task Post_WithDuplicateBook_ReturnsConflict()
    {
        // Arrange
        var existingBook = new Book
        {
            Id = "book1",
            BookName = "Same Book",
            AuthorId = "author1",
            Price = 20m,
            Category = "Fiction"
        };

        var request = new CreateBookRequest
        {
            BookName = "Same Book",
            Price = 20m,
            Category = "Fiction",
            AuthorId = "author1"
        };

        _authorsServiceMock.Setup(x => x.GetAsync("author1"))
            .ReturnsAsync(new Author { Id = "author1", AuthorName = "Test Author" });
        _booksServiceMock.Setup(x => x.GetByNameAndAuthorAsync("Same Book", "author1"))
            .ReturnsAsync(existingBook);

        // Act
        var result = await _controller.Post(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<BookResponse>>>(result);
        var conflictResult = Assert.IsType<ConflictObjectResult>(actionResult.Result);
        var apiResponse = Assert.IsType<ApiResponse<BookResponse>>(conflictResult.Value);

        Assert.False(apiResponse.Success);
        Assert.Contains("already exists", apiResponse.Message);
    }

    [Fact]
    public async Task Post_WithMissingAuthorInfo_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            BookName = "Test Book",
            Price = 30m,
            Category = "Horror"
        };

        // Act
        var result = await _controller.Post(request);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<BookResponse>>>(result);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var apiResponse = Assert.IsType<ApiResponse<BookResponse>>(badRequestResult.Value);

        Assert.False(apiResponse.Success);
        Assert.Contains("You must provide either an existing Author", apiResponse.Message);
    }
}