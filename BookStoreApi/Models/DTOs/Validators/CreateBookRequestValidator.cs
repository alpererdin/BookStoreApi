using FluentValidation;
using BookStoreApi.Models.DTOs.Requests;

namespace BookStoreApi.Models.DTOs.Validators;

public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookRequestValidator()
    {
        // BookName  
        RuleFor(x => x.BookName)
            .NotEmpty().WithMessage("Book name is required")
            .Length(1, 100).WithMessage("Book name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-.,':!?]+$")
            .WithMessage("Book name contains invalid characters");

        // Price  
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Price cannot exceed 10000");

        // Category  
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(BeValidCategory).WithMessage("Invalid category. Valid categories: Fiction, Science, Horror, Biography, History, Technology");

        // AuthorId 
        When(x => !string.IsNullOrWhiteSpace(x.AuthorId), () =>
        {
            RuleFor(x => x.AuthorId)
                .Length(24).WithMessage("AuthorId must be a valid MongoDB ObjectId (24 characters)");
        });

        // NewAuthorName 
        When(x => !string.IsNullOrWhiteSpace(x.NewAuthorName), () =>
        {
            RuleFor(x => x.NewAuthorName)
                .Length(2, 100).WithMessage("Author name must be between 2 and 100 characters");
        });

        // Custom rule: AuthorId veya NewAuthorName biri olmalı, ikisi birden olamaz
        RuleFor(x => x)
            .Must(HaveEitherAuthorIdOrNewAuthorName)
            .WithMessage("Either AuthorId or NewAuthorName must be provided")
            .Must(NotHaveBothAuthorIdAndNewAuthorName)
            .WithMessage("Cannot provide both AuthorId and NewAuthorName");
    }

    private bool BeValidCategory(string category)
    {
        var validCategories = new[] { "Fiction", "Science", "Horror", "Biography", "History", "Technology" };
        return validCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }

    private bool HaveEitherAuthorIdOrNewAuthorName(CreateBookRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.AuthorId) ||
               !string.IsNullOrWhiteSpace(request.NewAuthorName);
    }

    private bool NotHaveBothAuthorIdAndNewAuthorName(CreateBookRequest request)
    {
        return string.IsNullOrWhiteSpace(request.AuthorId) ||
               string.IsNullOrWhiteSpace(request.NewAuthorName);
    }
}