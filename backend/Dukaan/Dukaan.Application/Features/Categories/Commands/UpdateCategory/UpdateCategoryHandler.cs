using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryHandler(IRepository<Category> repository)
    : ICommandHandler<UpdateCategoryCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        
        if (category is null)
            return CategoryErrors.NotFound;

        category.Name = request.Name;
        category.Description = request.Description;

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
