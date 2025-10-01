using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Models.DTOs;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BooksService _booksService;
    private readonly AuthorsService _authorsService;

    public BooksController(BooksService booksService, AuthorsService authorsService) // <-- YENİ PARAMETRE
    {
        _booksService = booksService;
        _authorsService = authorsService;  
    }

    [HttpGet]
    public async Task<List<Book>> Get()=>
        await _booksService.GetAsync();


    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Book>> Get(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }
        return book;
    }
 
   [HttpPost]
public async Task<IActionResult> Post([FromBody] CreateBookRequest request)
{
    string authorId;

    if (!string.IsNullOrEmpty(request.NewAuthorName))
    {
        var existingAuthor = await _authorsService.GetByNameAsync(request.NewAuthorName);

        if (existingAuthor is not null)
        {
            authorId = existingAuthor.Id!;
        }
        else
        {
            var newAuthor = new Author { AuthorName = request.NewAuthorName };
            await _authorsService.CreateAsync(newAuthor);
            authorId = newAuthor.Id!;
        }
    }
    else if (!string.IsNullOrEmpty(request.AuthorId))
    {
        var author = await _authorsService.GetAsync(request.AuthorId);
        if (author is null)
        {
            return BadRequest("The provided AuthorId does not exist.");
        }
        authorId = request.AuthorId;
    }
    else
    {
        return BadRequest("You must provide either an existing AuthorId or a NewAuthorName.");
    }

    var existingBook = await _booksService.GetByNameAndAuthorAsync(request.BookName, authorId);
    if (existingBook is not null)
    {
        return Conflict("A book with the same name by this author already exists.");
    }

    var newBook = new Book
    {
        BookName = request.BookName,
        Price = request.Price,
        Category = request.Category,
        AuthorId = authorId
    };

    await _booksService.CreateAsync(newBook);

    return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
}

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Book updatedBook)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();

        }
        updatedBook.Id = book.Id;
        await _booksService.UpdateAsync(id, updatedBook);
        return NoContent();
    }


    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _booksService.GetAsync(id);
        if (book is null)
        {
            return NotFound();
        }
        await _booksService.RemoveAsync(id);

        return NoContent();
    }

}
