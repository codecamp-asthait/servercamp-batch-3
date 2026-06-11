using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Categories.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description, Guid? ParentCategoryId) : ICommand<ErrorOr<CategoryDto>>;
