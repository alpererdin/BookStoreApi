using FluentValidation;
using BookStoreApi.Models.DTOs.Requests;

namespace BookStoreApi.Models.DTOs.Validators;

public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookRequestValidator()
    {
        // En az bir alan  
        RuleFor(x => x)
            .Must(HaveAtLeastOneField)
            .WithMessage("At least one field must be provided for update");

        // BookName  
        When(x => !string.IsNullOrWhiteSpace(x.BookName), () =>
        {
            RuleFor(x => x.BookName)
                .Length(1, 100).WithMessage("Book name must be between 1 and 100 characters")
                .Matches(@"^[a-zA-Z0-9\s\-.,':!?]+$")
                .WithMessage("Book name contains invalid characters");
        });

        // Price  
        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price!.Value)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(10000).WithMessage("Price cannot exceed 10000");
        });

        // Category  
        When(x => !string.IsNullOrWhiteSpace(x.Category), () =>
        {
            RuleFor(x => x.Category)
                .Must(BeValidCategory!)
                .WithMessage("Invalid category. Valid categories: Fiction, Science, Horror, Biography, History, Technology");
        });

        // AuthorId  
        When(x => !string.IsNullOrWhiteSpace(x.AuthorId), () =>
        {
            RuleFor(x => x.AuthorId)
                .Length(24).WithMessage("AuthorId must be a valid MongoDB ObjectId (24 characters)");
        });
    }

    private bool HaveAtLeastOneField(UpdateBookRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.BookName) ||
               request.Price.HasValue ||
               !string.IsNullOrWhiteSpace(request.Category) ||
               !string.IsNullOrWhiteSpace(request.AuthorId);
    }

    private bool BeValidCategory(string category)
    {
        var validCategories = new[] { "Fiction", "Science", "Horror", "Biography", "History", "Technology" };
        return validCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }
}