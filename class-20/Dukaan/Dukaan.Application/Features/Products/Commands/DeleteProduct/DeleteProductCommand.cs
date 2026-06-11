using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : ICommand<bool>;
