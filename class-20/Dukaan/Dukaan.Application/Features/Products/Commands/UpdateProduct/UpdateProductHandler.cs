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

        product.Name = request.Request.Name;
        product.Description = request.Request.Description;
        product.Price = request.Request.Price;
        product.ImageUrl = request.Request.ImageUrl;
        product.StockQuantity = request.Request.StockQuantity;

        repository.Update(product);
        await repository.SaveChangesAsync();
        return true;
    }
}
