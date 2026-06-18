using FluentValidation;

namespace Dukaan.Application.Features.Merchants.Queries.CheckSlugAvailability;

public class CheckSlugAvailabilityQueryValidator : AbstractValidator<CheckSlugAvailabilityQuery>
{
    public CheckSlugAvailabilityQueryValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(100).WithMessage("Slug must not exceed 100 characters.");
    }
}
