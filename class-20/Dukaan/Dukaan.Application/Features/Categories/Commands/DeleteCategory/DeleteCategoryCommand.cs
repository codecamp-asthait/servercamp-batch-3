using Dukaan.Application.Core.Abstractions;

namespace Dukaan.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : ICommand<bool>;
