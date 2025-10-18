using System.Collections.Generic;
using System.Threading.Tasks;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces
{
    public interface IIngredientService
    {
        Task<IEnumerable<Ingredient>> GetAllAsync();
        Task<Ingredient?> GetByIdAsync(int id);
        Task<Ingredient> CreateAsync(Ingredient ingredient);
        Task<Ingredient?> UpdateAsync(int id, Ingredient ingredient);
        Task<bool> DeleteAsync(int id);

        // Приход ингредиента
        Task<IngredientArrival> AddArrivalAsync(IngredientArrival arrival);
        Task<IEnumerable<IngredientArrival>> GetArrivalsAsync(int ingredientId);
        Task<IngredientArrival?> GetArrivalByIdAsync(int arrivalId);

        // Поиск по имени
        Task<IEnumerable<Ingredient>> SearchAsync(string name);
    }
}