using Dukaan.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description) : ICommand<ErrorOr<Success>>;
