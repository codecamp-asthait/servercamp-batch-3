using FluentValidation;

namespace Dukaan.Application.Features.Categories.Queries.GetCategoriesByParent;

public class GetCategoriesByParentQueryValidator : AbstractValidator<GetCategoriesByParentQuery>
{
    public GetCategoriesByParentQueryValidator()
    {
        RuleFor(x => x.ParentId)
            .NotEmpty().WithMessage("Parent ID is required.");
    }
}
