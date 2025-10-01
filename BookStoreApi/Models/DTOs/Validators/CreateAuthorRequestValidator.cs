using FluentValidation;
using BookStoreApi.Models.DTOs.Requests;

namespace BookStoreApi.Models.DTOs.Validators;

public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(x => x.AuthorName)
            .NotEmpty().WithMessage("Author name is required")
            .Length(2, 100).WithMessage("Author name must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s.\-']+$").WithMessage("Author name can only contain letters, spaces, dots, hyphens and apostrophes");

        RuleFor(x => x.Biography)
            .MaximumLength(500).WithMessage("Biography cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Biography));

        RuleFor(x => x.BirthYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year)
            .WithMessage($"Birth year must be between 1900 and {DateTime.UtcNow.Year}")
            .When(x => x.BirthYear.HasValue);
    }
}