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
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Search term cannot be empty.");

        var ingredients = await _productService.SearchAsync(name);

        var result = ingredients.Select(p => new ProductGetDTO
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ProductCategoryId = p.ProductCategoryId,
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string imagePath = null;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "products");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            imagePath = $"/images/products/{uniqueFileName}";
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = imagePath,
            ProductCategoryId = dto.ProductCategoryId
        };

        await _productService.AddAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromForm] ProductDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _productService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        string imagePath = existing.ImageUrl;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "products");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(existing.ImageUrl))
            {
                var oldFile = Path.Combine(_env.WebRootPath, existing.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldFile))
                    System.IO.File.Delete(oldFile);
            }

            imagePath = $"/images/products/{uniqueFileName}";
        }

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.Price = dto.Price;
        existing.ImageUrl = imagePath;
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
            var filePath = Path.Combine(_env.WebRootPath, product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        await _productService.DeleteAsync(id);
        return NoContent();
    }
}