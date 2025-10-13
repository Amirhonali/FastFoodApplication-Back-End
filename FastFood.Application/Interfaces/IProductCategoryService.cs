using System;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IProductCategoryService
{
    Task<IEnumerable<ProductCategory>> GetAllAsync();
    Task<ProductCategory?> GetByIdAsync(int id);
    Task AddAsync(ProductCategory category);
    Task<ProductCategory?> UpdateAsync(int id, ProductCategory category);
    Task<bool> DeleteAsync(int id);
}