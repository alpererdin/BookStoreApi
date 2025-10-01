using BookStoreApi.Models;
using BookStoreApi.Models.DTOs.Requests;
using BookStoreApi.Models.DTOs.Responses;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Interfaces;
namespace BookStoreApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;
    private readonly IAuthorsService _authorsService;

    public BooksController(IBooksService booksService, IAuthorsService authorsService)
    {
        _booksService = booksService;
        _authorsService = authorsService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<BookResponse>>>> Get()
    {
        var books = await _booksService.GetAsync();

 
        var bookResponses = new List<BookResponse>();
        foreach (var book in books)
        {
            var author = await _authorsService.GetAsync(book.AuthorId);
            bookResponses.Add(new BookResponse
            {
                Id = book.Id!,
                BookName = book.BookName,
                Price = book.Price,
                Category = book.Category,
                AuthorId = book.AuthorId,
                AuthorName = author?.AuthorName // Yazar ismini ekle
            });
        }

        return Ok(ApiResponse<List<BookResponse>>.SuccessResponse(bookResponses));
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Get(string id)
    {
        var book = await _booksService.GetAsync(id);
        if (book is null)
        {
            return NotFound(ApiResponse<BookResponse>.ErrorResponse("Book not found"));
        }

        var author = await _authorsService.GetAsync(book.AuthorId);
        var response = new BookResponse
        {
            Id = book.Id!,
            BookName = book.BookName,
            Price = book.Price,
            Category = book.Category,
            AuthorId = book.AuthorId,
            AuthorName = author?.AuthorName
        };

        return Ok(ApiResponse<BookResponse>.SuccessResponse(response));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Post([FromBody] CreateBookRequest request)
    {
         

        string authorId;
        string authorName;
 
        if (!string.IsNullOrEmpty(request.NewAuthorName))
        {
            var existingAuthor = await _authorsService.GetByNameAsync(request.NewAuthorName);
            if (existingAuthor is not null)
            {
                authorId = existingAuthor.Id!;
                authorName = existingAuthor.AuthorName;
            }
            else
            {
                var newAuthor = new Author { AuthorName = request.NewAuthorName };
                await _authorsService.CreateAsync(newAuthor);
                authorId = newAuthor.Id!;
                authorName = newAuthor.AuthorName;
            }
        }
 
        else if (!string.IsNullOrEmpty(request.AuthorId))
        {
            var author = await _authorsService.GetAsync(request.AuthorId);
            if (author is null)
            {
                return BadRequest(ApiResponse<BookResponse>.ErrorResponse(
                    "The provided AuthorId does not exist."
                ));
            }
            authorId = request.AuthorId;
            authorName = author.AuthorName;
        }
        else
        {
 
            return BadRequest(ApiResponse<BookResponse>.ErrorResponse(
                "You must provide either an existing AuthorId or a NewAuthorName."
            ));
        }
 
        var existingBook = await _booksService.GetByNameAndAuthorAsync(request.BookName, authorId);
        if (existingBook is not null)
        {
            return Conflict(ApiResponse<BookResponse>.ErrorResponse(
                "A book with the same name by this author already exists."
            ));
        }
 
        var newBook = new Book
        {
            BookName = request.BookName,
            Price = request.Price,
            Category = request.Category,
            AuthorId = authorId
        };

        await _booksService.CreateAsync(newBook);

 
        var bookResponse = new BookResponse
        {
            Id = newBook.Id!,
            BookName = newBook.BookName,
            Price = newBook.Price,
            Category = newBook.Category,
            AuthorId = newBook.AuthorId,
            AuthorName = authorName   
        };

  
        return CreatedAtAction(
            nameof(Get),
            new { id = newBook.Id },
            ApiResponse<BookResponse>.SuccessResponse(bookResponse, "Book created successfully")
        );
    }

    [HttpPut("{id:length(24)}")]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Update(
        string id,
        [FromBody] UpdateBookRequest request)  
    {
        var book = await _booksService.GetAsync(id);
        if (book is null)
        {
            return NotFound(ApiResponse<BookResponse>.ErrorResponse("Book not found"));
        }
 
        if (!string.IsNullOrWhiteSpace(request.BookName))
            book.BookName = request.BookName;

        if (request.Price.HasValue)
            book.Price = request.Price.Value;

        if (!string.IsNullOrWhiteSpace(request.Category))
            book.Category = request.Category;

        if (!string.IsNullOrWhiteSpace(request.AuthorId))
        {
            var author = await _authorsService.GetAsync(request.AuthorId);
            if (author is null)
            {
                return BadRequest(ApiResponse<BookResponse>.ErrorResponse(
                    "The provided AuthorId does not exist."
                ));
            }
            book.AuthorId = request.AuthorId;
        }

        await _booksService.UpdateAsync(id, book);

        var author2 = await _authorsService.GetAsync(book.AuthorId);
        var response = new BookResponse
        {
            Id = book.Id!,
            BookName = book.BookName,
            Price = book.Price,
            Category = book.Category,
            AuthorId = book.AuthorId,
            AuthorName = author2?.AuthorName
        };

        return Ok(ApiResponse<BookResponse>.SuccessResponse(response, "Book updated successfully"));
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(string id)
    {
        var book = await _booksService.GetAsync(id);
        if (book is null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Book not found"));
        }

        await _booksService.RemoveAsync(id);

        return Ok(ApiResponse<object>.SuccessResponse(null!, "Book deleted successfully"));
    }
}