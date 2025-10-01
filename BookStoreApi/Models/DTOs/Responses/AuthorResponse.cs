namespace BookStoreApi.Models.DTOs.Responses;

public class AuthorResponse
{
    public string Id { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public string? Biography { get; set; }
    public int? BirthYear { get; set; }
    public int BookCount { get; set; }  
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}