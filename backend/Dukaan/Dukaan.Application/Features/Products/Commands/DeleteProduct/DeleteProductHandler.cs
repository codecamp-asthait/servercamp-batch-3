using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductHandler(IRepository<Product> repository)
    : ICommandHandler<DeleteProductCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, trackChanges: true, cancellationToken: cancellationToken);
        
        if (product is null)
            return ProductErrors.NotFound;

        repository.Remove(product);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
