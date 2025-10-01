using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Requests;

public class CreateBookRequest
{
    [Required(ErrorMessage = "Book name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Book name must be between 1 and 100 characters")]
    public required string BookName { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
    public required string Category { get; set; }

    public string? AuthorId { get; set; }

    [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters")]
    public string? NewAuthorName { get; set; }
}