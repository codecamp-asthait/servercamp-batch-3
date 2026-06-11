using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Products.Commands.DetachCategory;

public class DetachCategoryHandler(IRepository<CategorizedProduct> repository)
    : ICommandHandler<DetachCategoryCommand, bool>
{
    public async Task<bool> Handle(DetachCategoryCommand request, CancellationToken cancellationToken)
    {
        var associations = await repository.FindAsync(cp =>
            cp.ProductId == request.ProductId && cp.CategoryId == request.CategoryId);

        var association = associations.FirstOrDefault();
        if (association == null) return false;

        repository.Remove(association);
        await repository.SaveChangesAsync();
        return true;
    }
}
