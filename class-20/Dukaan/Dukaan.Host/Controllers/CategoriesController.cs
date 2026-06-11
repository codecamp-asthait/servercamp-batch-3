using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Categories.Commands.CreateCategory;
using Dukaan.Application.Features.Categories.Commands.DeleteCategory;
using Dukaan.Application.Features.Categories.Commands.UpdateCategory;
using Dukaan.Application.Features.Categories.Dtos;
using Dukaan.Application.Features.Categories.Queries.GetCategories;
using Dukaan.Application.Features.Categories.Queries.GetCategoryById;
using Dukaan.Application.Features.Categories.Queries.GetCategoriesByParent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Dukaan.Host.Controllers;

[Authorize]
public class CategoriesController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CategoryDto>>> GetAll([FromQuery] PaginationRequest request)
        => Ok(await Mediator.Send(new GetCategoriesQuery(request)));

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> Get(Guid id)
    {
        var category = await Mediator.Send(new GetCategoryByIdQuery(id));
        return category == null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand command)
        => await Mediator.Send(command with { Id = id }) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) =>
        await Mediator.Send(new DeleteCategoryCommand(id)) ? NoContent() : NotFound();

    [HttpGet("parent/{parentId}")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByParent(Guid parentId)
        => Ok(await Mediator.Send(new GetCategoriesByParentQuery(parentId)));
}
