using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Products;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductHandler(IRepository<Product> repository)
    : ICommandHandler<UpdateProductCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        
        if (product is null)
            return ProductErrors.NotFound;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;
        product.StockQuantity = request.StockQuantity;
        product.IsActive = request.IsActive;

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
