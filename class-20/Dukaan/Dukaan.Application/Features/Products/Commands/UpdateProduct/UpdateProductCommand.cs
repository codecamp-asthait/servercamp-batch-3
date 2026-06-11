using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;

namespace Dukaan.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(Guid Id, ProductRequestDto Request) : ICommand<bool>;
