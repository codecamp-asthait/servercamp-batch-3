using Dukaan.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.DetachCategory;

public record DetachCategoryCommand(Guid ProductId, Guid CategoryId) : ICommand<ErrorOr<Success>>;
