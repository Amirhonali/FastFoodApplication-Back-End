using FastFood.Application.DTOs.IngredientDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientController : ControllerBase
{
    private readonly IIngredientService _ingredientService;

    public IngredientController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ingredients = await _ingredientService.GetAllAsync();
        return Ok(ingredients);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ingredient = await _ingredientService.GetByIdAsync(id);
        if (ingredient == null)
            return NotFound();

        return Ok(ingredient);
    }

    [HttpPost]
    public async Task<IActionResult> Create(IngredientDTO dto)
    {
        var ingredient = new Ingredient
        {
            Name = dto.Name,
            IngredientCategory = dto.IngredientCategory,
            Quantity = dto.Quantity,
            Weight = dto.Weight,
            Volume = dto.Volume
        };

        await _ingredientService.CreateAsync(ingredient);
        return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, IngredientDTO dto)
    {
        var existing = await _ingredientService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.Name = dto.Name;
        existing.IngredientCategory = dto.IngredientCategory;
        existing.Quantity = dto.Quantity;
        existing.Weight = dto.Weight;
        existing.Volume = dto.Volume;

        await _ingredientService.UpdateAsync(existing);
        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
         var ingredient = await _ingredientService.GetByIdAsync(id);
        if (ingredient == null)
            return NotFound();

        await _ingredientService.DeleteAsync(id);
        return NoContent();
    }
}
