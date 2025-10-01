using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models.DTOs.Requests;

public class UpdateAuthorRequest
{
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2 and 100 characters")]
    public string? AuthorName { get; set; }

    [StringLength(500, ErrorMessage = "Biography cannot exceed 500 characters")]
    public string? Biography { get; set; }

    [Range(1900, 2100, ErrorMessage = "Birth year must be between 1900 and 2100")]
    public int? BirthYear { get; set; }
}