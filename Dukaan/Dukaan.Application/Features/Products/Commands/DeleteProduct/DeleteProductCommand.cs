using Dukaan.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : ICommand<ErrorOr<Deleted>>;
