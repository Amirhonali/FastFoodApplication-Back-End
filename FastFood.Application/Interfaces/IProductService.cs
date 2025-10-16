using System;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

using FastFood.Application.DTOs.ProductDTOs;
using FastFood.Domain.Entities;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product, List<ProductIngredientDTO>? ingredients = null);
    Task<Product?> UpdateAsync(int id, Product product, List<ProductIngredientDTO>? ingredients = null);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Product>> SearchAsync(string name);
}