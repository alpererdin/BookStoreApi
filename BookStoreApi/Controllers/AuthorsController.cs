using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly AuthorsService _authorsService;

    public AuthorsController(AuthorsService authorsService) =>
        _authorsService = authorsService;

    [HttpGet]
    public async Task<List<Author>> Get() =>
        await _authorsService.GetAsync();

    [HttpGet("{id:length(24)}")]

    public async Task<ActionResult<Author>> Get(string id)
    {
        var author = await _authorsService.GetAsync(id); 

        if (author is null)
        {
            return NotFound();
        }
        return author;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Author newAuthor)
    {
        await _authorsService.CreateAsync(newAuthor);
        return CreatedAtAction(nameof(Get), new { id = newAuthor.Id }, newAuthor);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Author updatedAuthor)
    {
        var author = await _authorsService.GetAsync(id);

        if (author is null)
        {
            return NotFound();
        }

        updatedAuthor.Id = author.Id;
        await _authorsService.UpdateAsync(id, updatedAuthor);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var author = await _authorsService.GetAsync(id);
        if (author is null)
        {
            return NotFound();
        }

        await _authorsService.RemoveAsync(id);
        return NoContent();
    }
}