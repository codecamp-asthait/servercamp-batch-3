using ErrorOr;

namespace Dukaan.Application.Features.Categories;

public static class CategoryErrors
{
    public static Error NotFound => Error.NotFound("Category.NotFound", "Category not found.");
    public static Error HasSubCategories => Error.Conflict("Category.HasSubCategories", "Cannot delete category with sub-categories.");
    public static Error HasProducts => Error.Conflict("Category.HasProducts", "Cannot delete category with linked products.");
    public static Error ParentNotFound => Error.NotFound("Category.ParentNotFound", "Parent category not found.");
}
