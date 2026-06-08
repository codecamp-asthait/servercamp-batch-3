using learning_validation_mediatr.DTOs;
using learning_validation_mediatr.Services;
using Microsoft.AspNetCore.Mvc;

namespace learning_validation_mediatr.Controllers;

/// <summary>REST API controller for product operations.</summary>
[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    /// <summary>GET api/product — returns all products.</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(productService.GetAll());

    /// <summary>GET api/product/{id} — returns a single product by ID.</summary>
    /// <param name="id">The product ID.</param>
    /// <response code="200">Product found.</response>
    /// <response code="404">Product not found.</response>
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = productService.GetById(id);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>POST api/product — creates a new product.</summary>
    /// <param name="dto">Product data. Validated via <see cref="ProductCreateDto"/> annotations.</param>
    /// <response code="201">Product created successfully.</response>
    /// <response code="400">Validation failed — see response body for errors.</response>
    [HttpPost]
    public IActionResult Create([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = productService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}
