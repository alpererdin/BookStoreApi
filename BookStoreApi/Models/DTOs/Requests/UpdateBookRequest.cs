using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Requests;

public class UpdateBookRequest
{
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Book name must be between 1 and 100 characters")]
    public string? BookName { get; set; }

    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    public decimal? Price { get; set; }

    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public string? Category { get; set; }

    public string? AuthorId { get; set; }
}