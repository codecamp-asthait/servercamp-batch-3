using FluentValidation;
using learning_validation_mediatr.DTOs;

namespace learning_validation_mediatr.Validators;

/// <summary>
/// FluentValidation validator for <see cref="ProductCreateDto"/>.
/// Replaces annotation-based validation; registered automatically via assembly scanning.
/// </summary>
public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
{
    /// <summary>Defines validation rules for <see cref="ProductCreateDto"/>.</summary>
    public ProductCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
}
