namespace BookStoreApi.Models.DTOs.Responses;

public class BookResponse
{
    public string Id { get; set; } = null!;
    public string BookName { get; set; } = null!;
    public decimal Price { get; set; }
    public string Category { get; set; } = null!;
    public string AuthorId { get; set; } = null!;
    public string? AuthorName { get; set; }  
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}