using FastFood.Application.DTOs.ProductCategoryDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FastFood.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductCategoryController : ControllerBase
{
    private readonly IProductCategoryService _productCategoryService;

    public ProductCategoryController(IProductCategoryService productCategoryService)
    {
        _productCategoryService = productCategoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _productCategoryService.GetAllAsync();

        var result = categories.Select(c => new ProductCatGetDTO
        {
            Name = c.Name
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _productCategoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        var dto = new ProductCatGetDTO
        {
            Name = category.Name
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCatCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var category = new ProductCategory
        {
            Name = dto.Name
        };

        await _productCategoryService.AddAsync(category);

        var result = new ProductCatGetDTO { Name = category.Name };

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductCatUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _productCategoryService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.Name = dto.Name;

        var updated = await _productCategoryService.UpdateAsync(id, existing);
        var result = new ProductCatGetDTO { Name = updated!.Name };

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productCategoryService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}