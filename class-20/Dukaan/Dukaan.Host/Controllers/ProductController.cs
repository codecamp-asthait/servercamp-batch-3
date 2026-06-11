using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Commands.AttachCategory;
using Dukaan.Application.Features.Products.Commands.CreateProduct;
using Dukaan.Application.Features.Products.Commands.DeleteProduct;
using Dukaan.Application.Features.Products.Commands.DetachCategory;
using Dukaan.Application.Features.Products.Commands.UpdateProduct;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Features.Products.Queries.GetProductById;
using Dukaan.Application.Features.Products.Queries.GetProducts;
using Dukaan.Application.Features.Products.Queries.GetProductsByCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Authorize]
public class ProductController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetAll(
        [FromQuery] PaginationRequest request)
        => Ok(await Mediator.Send(new GetProductsQuery(request)));

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetByCategory(Guid categoryId,
        [FromQuery] PaginationRequest request)
        => Ok(await Mediator.Send(new GetProductsByCategoryQuery(categoryId, request)));

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(Guid id)
    {
        var product = await Mediator.Send(new GetProductByIdQuery(id));
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductCommand command)
        => Ok(await Mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductCommand command)
        => await Mediator.Send(command with { Id = id }) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        => await Mediator.Send(new DeleteProductCommand(id)) ? NoContent() : NotFound();

    [HttpPost("{id}/categories/{categoryId}")]
    public async Task<IActionResult> AttachCategory(Guid id, Guid categoryId)
        => await Mediator.Send(new AttachCategoryCommand(id, categoryId)) ? NoContent() : NotFound();

    [HttpDelete("{id}/categories/{categoryId}")]
    public async Task<IActionResult> DetachCategory(Guid id, Guid categoryId)
        => await Mediator.Send(new DetachCategoryCommand(id, categoryId)) ? NoContent() : NotFound();
}
