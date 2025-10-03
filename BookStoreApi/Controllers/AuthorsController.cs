using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Models.DTOs.Requests;
using BookStoreApi.Models.DTOs.Responses;
using BookStoreApi.Interfaces;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorsService _authorsService;
    private readonly IBooksService _booksService;

    public AuthorsController(IAuthorsService authorsService, IBooksService booksService)
    {
        _authorsService = authorsService;
        _booksService = booksService;
    }

     
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AuthorResponse>>>> Get()
    {
        var authors = await _authorsService.GetAsync();

        var authorResponses = new List<AuthorResponse>();
        foreach (var author in authors)
        {
        
            var books = await _booksService.GetAsync();
            var bookCount = books.Count(b => b.AuthorId == author.Id);

            authorResponses.Add(new AuthorResponse
            {
                Id = author.Id!,
                AuthorName = author.AuthorName,
                Biography = author.Biography,
                BirthYear = author.BirthYear,
                BookCount = bookCount
            });
        }

        return Ok(ApiResponse<List<AuthorResponse>>.SuccessResponse(authorResponses));
    }

  
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<ApiResponse<AuthorResponse>>> Get(string id)
    {
        var author = await _authorsService.GetAsync(id);

        if (author is null)
        {
            return NotFound(ApiResponse<AuthorResponse>.ErrorResponse("Author not found"));
        }
         
        var books = await _booksService.GetAsync();
        var bookCount = books.Count(b => b.AuthorId == author.Id);

        var response = new AuthorResponse
        {
            Id = author.Id!,
            AuthorName = author.AuthorName,
            Biography = author.Biography,
            BirthYear = author.BirthYear,
            BookCount = bookCount
        };

        return Ok(ApiResponse<AuthorResponse>.SuccessResponse(response));
    }

    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AuthorResponse>>> Post([FromBody] CreateAuthorRequest request)
    {
      
        var existingAuthor = await _authorsService.GetByNameAsync(request.AuthorName);
        if (existingAuthor is not null)
        {
            return Conflict(ApiResponse<AuthorResponse>.ErrorResponse(
                $"An author with the name '{request.AuthorName}' already exists."
            ));
        }

        var newAuthor = new Author
        {
            AuthorName = request.AuthorName,
            Biography = request.Biography,
            BirthYear = request.BirthYear
        };

        await _authorsService.CreateAsync(newAuthor);

        var response = new AuthorResponse
        {
            Id = newAuthor.Id!,
            AuthorName = newAuthor.AuthorName,
            Biography = newAuthor.Biography,
            BirthYear = newAuthor.BirthYear,
            BookCount = 0 
        };

        return CreatedAtAction(
            nameof(Get),
            new { id = newAuthor.Id },
            ApiResponse<AuthorResponse>.SuccessResponse(response, "Author created successfully")
        );
    }

    
    [HttpPut("{id:length(24)}")]
    public async Task<ActionResult<ApiResponse<AuthorResponse>>> Update(
        string id,
        [FromBody] UpdateAuthorRequest request)
    {
        var author = await _authorsService.GetAsync(id);

        if (author is null)
        {
            return NotFound(ApiResponse<AuthorResponse>.ErrorResponse("Author not found"));
        }
         
        if (!string.IsNullOrWhiteSpace(request.AuthorName))
        {
           
            if (request.AuthorName != author.AuthorName)
            {
                var existingAuthor = await _authorsService.GetByNameAsync(request.AuthorName);
                if (existingAuthor is not null)
                {
                    return Conflict(ApiResponse<AuthorResponse>.ErrorResponse(
                        $"An author with the name '{request.AuthorName}' already exists."
                    ));
                }
            }
            author.AuthorName = request.AuthorName;
        }

        if (!string.IsNullOrWhiteSpace(request.Biography))
            author.Biography = request.Biography;

        if (request.BirthYear.HasValue)
            author.BirthYear = request.BirthYear;

        await _authorsService.UpdateAsync(id, author);
 
        var books = await _booksService.GetAsync();
        var bookCount = books.Count(b => b.AuthorId == author.Id);

        var response = new AuthorResponse
        {
            Id = author.Id!,
            AuthorName = author.AuthorName,
            Biography = author.Biography,
            BirthYear = author.BirthYear,
            BookCount = bookCount
        };

        return Ok(ApiResponse<AuthorResponse>.SuccessResponse(response, "Author updated successfully"));
    }

  
    [HttpDelete("{id:length(24)}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(string id)
    {
        var author = await _authorsService.GetAsync(id);

        if (author is null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Author not found"));
        }

     
        var books = await _booksService.GetAsync();
        var authorBooks = books.Where(b => b.AuthorId == id).ToList();

        if (authorBooks.Any())
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                $"Cannot delete author. There are {authorBooks.Count} book(s) associated with this author. Please delete or reassign the books first.",
                new List<string> { $"Books: {string.Join(", ", authorBooks.Select(b => b.BookName))}" }
            ));
        }

        await _authorsService.RemoveAsync(id);

        return Ok(ApiResponse<object>.SuccessResponse(null!, "Author deleted successfully"));
    }
}