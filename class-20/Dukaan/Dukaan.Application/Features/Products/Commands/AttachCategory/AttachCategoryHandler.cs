using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;

namespace Dukaan.Application.Features.Products.Commands.AttachCategory;

public class AttachCategoryHandler(IRepository<CategorizedProduct> repository)
    : ICommandHandler<AttachCategoryCommand, bool>
{
    public async Task<bool> Handle(AttachCategoryCommand request, CancellationToken cancellationToken)
    {
        await repository.AddAsync(new CategorizedProduct
        {
            ProductId = request.ProductId,
            CategoryId = request.CategoryId
        });

        await repository.SaveChangesAsync();
        return true;
    }
}
