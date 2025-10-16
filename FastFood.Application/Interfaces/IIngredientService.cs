using System;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IIngredientService
{
    Task<IEnumerable<Ingredient>> GetAllAsync();
    Task<Ingredient?> GetByIdAsync(int id);
    Task<Ingredient> CreateAsync(Ingredient ingredient);
    Task<Ingredient?> UpdateAsync(int id, Ingredient ingredient);
    Task<bool> DeleteAsync(int id);

    // arrival / prixod
    Task<IngredientArrival> AddArrivalAsync(IngredientArrival arrival);
    Task<IEnumerable<IngredientArrival>> GetArrivalsAsync(int ingredientId);
    Task<IEnumerable<Ingredient>> SearchAsync(string name);
}