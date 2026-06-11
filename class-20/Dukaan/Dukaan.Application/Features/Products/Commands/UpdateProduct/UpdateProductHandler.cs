using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductHandler(IRepository<Product> repository)
    : ICommandHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, trackChanges: true);
        if (product == null) return false;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;
        product.StockQuantity = request.StockQuantity;

        repository.Update(product);
        await repository.SaveChangesAsync();
        return true;
    }
}
