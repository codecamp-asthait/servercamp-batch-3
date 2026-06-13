using Dukaan.Application.Dtos;
using Dukaan.Application.Features.Products.Commands.AttachCategory;
using Dukaan.Application.Features.Products.Commands.CreateProduct;
using Dukaan.Application.Features.Products.Commands.DeleteProduct;
using Dukaan.Application.Features.Products.Commands.DetachCategory;
using Dukaan.Application.Features.Products.Commands.UpdateProduct;
using Dukaan.Application.Features.Products.Dtos;
using Dukaan.Application.Features.Products.Queries.GetActiveProducts;
using Dukaan.Application.Features.Products.Queries.GetActiveProductsByCategory;
using Dukaan.Application.Features.Products.Queries.GetProductById;
using Dukaan.Application.Features.Products.Queries.GetProducts;
using Dukaan.Application.Features.Products.Queries.GetProductsByCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

[Authorize]
public class ProductsController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetAll([FromQuery] PaginationRequest request)
        => ToActionResult(await Mediator.Send(new GetProductsQuery(request)));

    [HttpGet("active")]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetActive([FromQuery] PaginationRequest request)
        => ToActionResult(await Mediator.Send(new GetActiveProductsQuery(request)));

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
        => ToActionResult(await Mediator.Send(new GetProductByIdQuery(id)));

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductCommand command)
        => ToActionResult(await Mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductCommand command)
        => ToActionResult(await Mediator.Send(command with { Id = id }));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await Mediator.Send(new DeleteProductCommand(id)));

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetByCategory(Guid categoryId, [FromQuery] PaginationRequest request)
        => ToActionResult(await Mediator.Send(new GetProductsByCategoryQuery(categoryId, request)));

    [HttpGet("category/{categoryId}/active")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetActiveByCategory(Guid categoryId, [FromQuery] PaginationRequest request)
        => ToActionResult(await Mediator.Send(new GetActiveProductsByCategoryQuery(categoryId, request)));

    [HttpPost("{productId}/categories/{categoryId}")]
    public async Task<IActionResult> AttachCategory(Guid productId, Guid categoryId)
        => ToActionResult(await Mediator.Send(new AttachCategoryCommand(productId, categoryId)));

    [HttpDelete("{productId}/categories/{categoryId}")]
    public async Task<IActionResult> DetachCategory(Guid productId, Guid categoryId)
        => ToActionResult(await Mediator.Send(new DetachCategoryCommand(productId, categoryId)));
}
