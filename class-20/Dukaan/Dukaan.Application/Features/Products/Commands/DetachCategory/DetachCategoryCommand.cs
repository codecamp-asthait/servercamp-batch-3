using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Products.Commands.DetachCategory;

public record DetachCategoryCommand(Guid ProductId, Guid CategoryId) : ICommand<bool>;
