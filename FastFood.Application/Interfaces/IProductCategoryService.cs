using System;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategory>> GetAllAsync();
        Task<ProductCategory?> GetByIdAsync(int id);
        Task AddAsync(ProductCategory category);
        Task UpdateAsync(ProductCategory category);
        Task DeleteAsync(int id);
    }