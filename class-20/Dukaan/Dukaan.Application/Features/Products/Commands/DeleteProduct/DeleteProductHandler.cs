using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductHandler(IRepository<Product> repository)
    : ICommandHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, trackChanges: true);
        if (product == null) return false;

        product.IsActive = false;
        repository.Update(product);
        await repository.SaveChangesAsync();
        return true;
    }
}
