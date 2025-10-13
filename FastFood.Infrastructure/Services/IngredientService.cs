using System;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Services;

public class IngredientService : IIngredientService
{
    private readonly IRepository<Ingredient> _repository;
    private readonly AppDbContext _context;

    public IngredientService(IRepository<Ingredient> repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<IEnumerable<Ingredient>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Ingredient?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Ingredient>> SearchAsync(string name)
    {
        return await _context.Ingredients
            .Where(p => EF.Functions.Like(p.Name, $"%{name}%"))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task CreateAsync(Ingredient ingredient)
    {
        if (string.IsNullOrWhiteSpace(ingredient.Name))
            throw new ArgumentException("Ingredient name cannot be empty");

        await _repository.AddAsync(ingredient);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ingredient ingredient)
    {
        var existing = await _repository.GetByIdAsync(ingredient.Id);
        if (existing is null)
            throw new KeyNotFoundException($"Ingredient with ID {ingredient.Id} not found");

        existing.Name = ingredient.Name;
        _repository.Update(existing);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            throw new KeyNotFoundException($"Ingredient with ID {id} not found");

        _repository.Remove(existing);
        await _repository.SaveChangesAsync();
    }
}