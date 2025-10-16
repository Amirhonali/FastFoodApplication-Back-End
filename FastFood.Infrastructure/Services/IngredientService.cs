using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastFood.Application.DTOs.IngredientDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Services;

public class IngredientService : IIngredientService
{
    private readonly AppDbContext _context;

    public IngredientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ingredient>> GetAllAsync()
    {
        return await _context.Ingredients
            .Include(i => i.IngredientArrivals)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Ingredient?> GetByIdAsync(int id)
    {
        return await _context.Ingredients
            .Include(i => i.IngredientArrivals)
            .Include(i => i.ProductIngredients)
                .ThenInclude(pi => pi.Product)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Ingredient> CreateAsync(Ingredient ingredient)
    {
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }

    public async Task<Ingredient?> UpdateAsync(int id, Ingredient ingredient)
    {
        var existing = await _context.Ingredients.FindAsync(id);
        if (existing == null) return null;

        existing.Name = ingredient.Name;
        existing.IngredientCategory = ingredient.IngredientCategory;
        existing.Quantity = ingredient.Quantity;
        existing.Weight = ingredient.Weight;
        existing.Volume = ingredient.Volume;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Ingredients.FindAsync(id);
        if (existing == null) return false;

        _context.Ingredients.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    // PRICE/ARRIVAL: add arrival and update on-hand quantity
    public async Task<IngredientArrival> AddArrivalAsync(IngredientArrival arrival)
    {
        var ing = await _context.Ingredients.FindAsync(arrival.IngredientId);
        if (ing == null) throw new KeyNotFoundException("Ingredient not found");

        _context.IngredientArrivals.Add(arrival);

        // update on-hand quantity (we use decimal quantities - convert if needed)
        var current = ing.Quantity; 
        ing.Quantity = current + arrival.Quantity;

        await _context.SaveChangesAsync();
        return arrival;
    }

    public async Task<IEnumerable<IngredientArrival>> GetArrivalsAsync(int ingredientId)
    {
        return await _context.IngredientArrivals
            .Where(a => a.IngredientId == ingredientId)
            .OrderByDescending(a => a.ArrivalDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Ingredient>> SearchAsync(string name)
    {
        return await _context.Ingredients
            .Where(i => EF.Functions.Like(i.Name, $"%{name}%"))
            .AsNoTracking()
            .ToListAsync();
    }
}