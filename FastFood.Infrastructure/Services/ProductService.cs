using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastFood.Application.DTOs.ProductDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product, List<ProductIngredientDTO>? ingredients = null)
    {
        if (ingredients != null && ingredients.Any())
        {
            foreach (var it in ingredients)
            {
                product.ProductIngredients.Add(new ProductIngredient
                {
                    IngredientId = it.IngredientId,
                    Quantity = it.Quantity
                });
            }
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product, List<ProductIngredientDTO>? ingredients = null)
    {
        var existing = await _context.Products
            .Include(p => p.ProductIngredients)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing == null) return null;

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.ImageUrl = product.ImageUrl;
        existing.ProductCategoryId = product.ProductCategoryId;

        if (ingredients != null)
        {
            // remove old relations
            _context.ProductIngredients.RemoveRange(existing.ProductIngredients);
            existing.ProductIngredients.Clear();

            foreach (var it in ingredients)
            {
                existing.ProductIngredients.Add(new ProductIngredient
                {
                    IngredientId = it.IngredientId,
                    Quantity = it.Quantity
                });
            }
        }

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await _context.Products.FindAsync(id);
        if (e == null) return false;
        _context.Products.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> SearchAsync(string name)
    {
        return await _context.Products
            .Where(p => EF.Functions.Like(p.Name, $"%{name}%"))
            .Include(p => p.ProductCategory)
            .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.Ingredient)
            .AsNoTracking()
            .ToListAsync();
    }
}