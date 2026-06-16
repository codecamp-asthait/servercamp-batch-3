using Dukaan.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dukaan.Application.Features.Categories.Dtos;
using Dukaan.Application.Features.Categories.Commands.DeleteCategory;
using Dukaan.Application.Features.Categories.Commands.CreateCategory;
using Dukaan.Application.Features.Categories.Commands.UpdateCategory;
using Dukaan.Application.Features.Categories.Queries.GetCategories;
using Dukaan.Application.Features.Categories.Queries.GetCategoryById;
using Dukaan.Application.Features.Categories.Queries.GetCategoriesByParent;
using Dukaan.Application.Features.Categories.Queries.GetCategoriesDropdown;

namespace Dukaan.Host.Controllers;

[Authorize]
public class CategoriesController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<CategoryDto>>> GetAll([FromQuery] PaginationRequest request)
        => ToActionResult(await Mediator.Send(new GetCategoriesQuery(request)));

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> Get(Guid id)
        => ToActionResult(await Mediator.Send(new GetCategoryByIdQuery(id)));

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryCommand command)
        => ToActionResult(await Mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand command)
        => ToActionResult(await Mediator.Send(command with { Id = id }));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await Mediator.Send(new DeleteCategoryCommand(id)));

    [HttpGet("dropdown")]
    public async Task<ActionResult<IEnumerable<CategoryDropdownDto>>> GetDropdown()
        => ToActionResult(await Mediator.Send(new GetCategoriesDropdownQuery()));

    [HttpGet("parent/{parentId}")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByParent(Guid parentId)
        => ToActionResult(await Mediator.Send(new GetCategoriesByParentQuery(parentId)));
}
