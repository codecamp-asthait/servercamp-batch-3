using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Dtos;

namespace Dukaan.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(ProductRequestDto Request) : ICommand<Features.Products.Dtos.ProductDto>;
