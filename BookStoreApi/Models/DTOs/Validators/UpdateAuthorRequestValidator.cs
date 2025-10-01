using FluentValidation;
using BookStoreApi.Models.DTOs.Requests;

namespace BookStoreApi.Models.DTOs.Validators;

public class UpdateAuthorRequestValidator : AbstractValidator<UpdateAuthorRequest>
{
    public UpdateAuthorRequestValidator()
    {
        // En az bir alan dolu olmalı
        RuleFor(x => x)
            .Must(HaveAtLeastOneField)
            .WithMessage("At least one field must be provided for update");

        RuleFor(x => x.AuthorName)
            .Length(2, 100).WithMessage("Author name must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s.\-']+$").WithMessage("Author name can only contain letters, spaces, dots, hyphens and apostrophes")
            .When(x => !string.IsNullOrWhiteSpace(x.AuthorName));

        RuleFor(x => x.Biography)
            .MaximumLength(500).WithMessage("Biography cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Biography));

        RuleFor(x => x.BirthYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year)
            .WithMessage($"Birth year must be between 1900 and {DateTime.UtcNow.Year}")
            .When(x => x.BirthYear.HasValue);
    }

    private bool HaveAtLeastOneField(UpdateAuthorRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.AuthorName) ||
               !string.IsNullOrWhiteSpace(request.Biography) ||
               request.BirthYear.HasValue;
    }
}