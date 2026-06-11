using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description, Guid? ParentCategoryId) : ICommand<bool>;
