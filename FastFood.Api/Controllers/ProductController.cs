using FastFood.Application.DTOs.ProductDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IWebHostEnvironment _env;

    public ProductController(IProductService productService, IWebHostEnvironment env)
    {
        _productService = productService;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();

        var result = products.Select(p => new ProductGetDTO
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            ImageUrl = p.ImageUrl,
            ProductCategoryId = p.ProductCategoryId,
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        var dto = new ProductGetDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
        };

        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Search term cannot be empty.");

        var products = await _productService.SearchAsync(name);

        var result = products.Select(p => new ProductGetDTO
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            ImageUrl = p.ImageUrl,
            ProductCategoryId = p.ProductCategoryId,
        });

        return Ok(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] ProductDTO dto)
    {
        if (dto.Image == null)
            return BadRequest("Image is required.");

        var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.Image.CopyToAsync(stream);
        }

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            ImageUrl = $"Product/images/{fileName}",
            ProductCategoryId = dto.ProductCategoryId
        };

        await _productService.AddAsync(product);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // Hozirgi method:
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromForm] ProductDTO dto)
    {
        var existing = await _productService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        if (dto.Image != null)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "Product/images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            existing.ImageUrl = $"/Product/images/{fileName}";
        }

        existing.Name = dto.Name;
        existing.Price = dto.Price;
        existing.Description = dto.Description;
        existing.ProductCategoryId = dto.ProductCategoryId;

        await _productService.UpdateAsync(id, existing);

        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}