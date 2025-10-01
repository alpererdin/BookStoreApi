namespace BookStoreApi.Models.DTOs;

public class CreateBookRequest
{

    public required string BookName { get; set; }
    public decimal Price { get; set; }
    public required string Category { get; set; }

    public string? AuthorId { get; set; }     
    public string? NewAuthorName { get; set; }
}