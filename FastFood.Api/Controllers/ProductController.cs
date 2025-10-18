using FastFood.Application.DTOs.ProductDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IWebHostEnvironment _env;

    public ProductController(IProductService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    // ✅ Получить все продукты
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAllAsync();
        return Ok(products);
    }

    // ✅ Получить продукт по Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    // ✅ Поиск по имени
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        var products = await _service.SearchAsync(name);
        return Ok(products);
    }

    // ✅ Создать продукт (с изображением и ингредиентами)
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string? imageUrl = null;

        // 📸 если есть изображение — сохраняем
        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await dto.ImageFile.CopyToAsync(stream);

            imageUrl = $"/uploads/{fileName}";
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = imageUrl ?? string.Empty,
            ProductCategoryId = dto.ProductCategoryId
        };

        var result = await _service.CreateAsync(product, dto.Ingredients);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // ✅ Обновить продукт
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDTO dto)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Product not found" });

        string? imageUrl = existing.ImageUrl;

        // 📸 обновляем изображение, если передано новое
        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await dto.ImageFile.CopyToAsync(stream);

            imageUrl = $"/uploads/{fileName}";
        }

        var updatedProduct = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = imageUrl ?? string.Empty,
            ProductCategoryId = dto.ProductCategoryId
        };

        var result = await _service.UpdateAsync(id, updatedProduct, dto.Ingredients);
        return Ok(result);
    }

    // ✅ Удалить продукт
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(new { message = "Product not found" });

        return Ok(new { message = "Product deleted successfully" });
    }
}