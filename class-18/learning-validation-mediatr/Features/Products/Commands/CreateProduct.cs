using FluentValidation;
using MediatR;

namespace learning_validation_mediatr.Features.Products.Commands;

/// <summary>Command to create a new product. Bound directly from the HTTP request body.</summary>
/// <param name="Name">Name of the product (2–100 characters, required).</param>
/// <param name="Price">Price of the product. Must be greater than 0.</param>
public record CreateProductCommand(string Name, decimal Price) : IRequest<Product>;

/// <summary>
/// FluentValidation validator for <see cref="CreateProductCommand"/>.
/// Executed by the MediatR <c>ValidationBehavior</c> pipeline before the handler runs.
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    /// <summary>Defines validation rules for <see cref="CreateProductCommand"/>.</summary>
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
}

/// <summary>Handles <see cref="CreateProductCommand"/> by creating and storing a new product.</summary>
public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product>
{
    /// <inheritdoc/>
    public Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = ProductStore.Products.Count + 1,
            Name = request.Name,
            Price = request.Price
        };
        ProductStore.Products.Add(product);
        return Task.FromResult(product);
    }
}
