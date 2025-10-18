using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Services
{
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
                .AsNoTracking()
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

        // ✅ Добавление прихода + обновление остатков
        public async Task<IngredientArrival> AddArrivalAsync(IngredientArrival arrival)
        {
            var ingredient = await _context.Ingredients.FindAsync(arrival.IngredientId);
            if (ingredient == null)
                throw new KeyNotFoundException($"Ingredient with id={arrival.IngredientId} not found");

            // обновляем остаток
            ingredient.Quantity += arrival.Quantity;

            _context.IngredientArrivals.Add(arrival);
            await _context.SaveChangesAsync();

            // 🔁 подгружаем связанные данные для возврата
            await _context.Entry(arrival)
                .Reference(a => a.Ingredient)
                .LoadAsync();

            return arrival;
        }

        public async Task<IEnumerable<IngredientArrival>> GetArrivalsAsync(int ingredientId)
        {
            return await _context.IngredientArrivals
                .Where(a => a.IngredientId == ingredientId)
                .Include(a => a.Ingredient)
                .AsNoTracking()
                .OrderByDescending(a => a.ArrivalDate)
                .ToListAsync();
        }

        // ✅ Новый метод
        public async Task<IngredientArrival?> GetArrivalByIdAsync(int arrivalId)
        {
            return await _context.IngredientArrivals
                .Include(a => a.Ingredient)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == arrivalId);
        }

        public async Task<IEnumerable<Ingredient>> SearchAsync(string name)
        {
            return await _context.Ingredients
                .Where(i => EF.Functions.Like(i.Name, $"%{name}%"))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}