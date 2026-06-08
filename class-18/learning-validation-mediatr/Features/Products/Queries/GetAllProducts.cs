using MediatR;

namespace learning_validation_mediatr.Features.Products.Queries;

/// <summary>Query to retrieve all products from the store.</summary>
public record GetAllProductsQuery : IRequest<IEnumerable<Product>>;

/// <summary>Handles <see cref="GetAllProductsQuery"/> by returning all products.</summary>
public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
{
    /// <inheritdoc/>
    public Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        => Task.FromResult<IEnumerable<Product>>(ProductStore.Products);
}
