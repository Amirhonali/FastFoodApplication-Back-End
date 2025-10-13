using System;
using FastFood.Application.DTOs.IngredientDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        var result = ingredients.Select(i => new IngredientGetDTO
        {
            Id = i.Id,
            Name = i.Name,
            Quantity = i.Quantity,
            Weight = i.Weight,
            Volume = i.Volume
        });
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ingredient = await _ingredientService.GetByIdAsync(id);
        if (ingredient == null)
            return NotFound();

        var dto = new IngredientGetDTO
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            Weight = ingredient.Weight,
            Volume = ingredient.Volume
        };

        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Search term cannot be empty.");

        var ingredients = await _ingredientService.SearchAsync(name);

        var result = ingredients.Select(i => new IngredientGetDTO
        {
            Id = i.Id,
            Name = i.Name,
            Quantity = i.Quantity,
            Weight = i.Weight,
            Volume = i.Volume
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IngredientDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ingredient = new Ingredient
        {
            Name = dto.Name,
            IngredientCategory = dto.IngredientCategory,
            Quantity = dto.Quantity,
            Weight = dto.Weight,
            Volume = dto.Volume
        };

        await _ingredientService.CreateAsync(ingredient);

        var result = new IngredientGetDTO
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            Weight = ingredient.Weight,
            Volume = ingredient.Volume
        };

        return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] IngredientDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

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
