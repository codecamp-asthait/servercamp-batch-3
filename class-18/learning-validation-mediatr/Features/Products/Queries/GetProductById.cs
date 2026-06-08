using MediatR;

namespace learning_validation_mediatr.Features.Products.Queries;

/// <summary>Query to retrieve a single product by its ID.</summary>
/// <param name="Id">The ID of the product to retrieve.</param>
public record GetProductByIdQuery(int Id) : IRequest<Product?>;

/// <summary>Handles <see cref="GetProductByIdQuery"/> by looking up the product by ID.</summary>
public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    /// <inheritdoc/>
    public Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        => Task.FromResult(ProductStore.Products.FirstOrDefault(p => p.Id == request.Id));
}
