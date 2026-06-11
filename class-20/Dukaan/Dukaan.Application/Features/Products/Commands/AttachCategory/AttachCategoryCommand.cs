using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Products.Commands.AttachCategory;

public record AttachCategoryCommand(Guid ProductId, Guid CategoryId) : ICommand<bool>;
