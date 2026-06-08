using learning_validation_mediatr.Features.Products.Commands;
using learning_validation_mediatr.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace learning_validation_mediatr.Controllers;

/// <summary>REST API controller for product operations. Dispatches requests via MediatR.</summary>
[ApiController]
[Route("api/[controller]")]
public class ProductController(IMediator mediator) : ControllerBase
{
    /// <summary>GET api/product — returns all products.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await mediator.Send(new GetAllProductsQuery()));

    /// <summary>GET api/product/{id} — returns a single product by ID.</summary>
    /// <param name="id">The product ID.</param>
    /// <response code="200">Product found.</response>
    /// <response code="404">Product not found.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await mediator.Send(new GetProductByIdQuery(id));
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>POST api/product — creates a new product.</summary>
    /// <remarks>
    /// Validated by <see cref="learning_validation_mediatr.Behaviors.ValidationBehavior{TRequest,TResponse}"/>
    /// in the MediatR pipeline. Exceptions are handled globally by
    /// <see cref="learning_validation_mediatr.Middleware.GlobalExceptionHandler"/>.
    /// </remarks>
    /// <param name="command">Create product command.</param>
    /// <response code="201">Product created successfully.</response>
    /// <response code="400">Validation failed — see response body for errors.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var product = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}
