using Dukaan.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Application.Features.Products.Commands.AttachCategory;

public record AttachCategoryCommand(Guid ProductId, Guid CategoryId) : ICommand<ErrorOr<Success>>;
