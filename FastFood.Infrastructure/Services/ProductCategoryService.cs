using System;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;

namespace FastFood.Infrastructure.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IRepository<ProductCategory> _categoryRepository;

    public ProductCategoryService(IRepository<ProductCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<ProductCategory>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<ProductCategory?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task AddAsync(ProductCategory category)
    {
        if (string.IsNullOrWhiteSpace(category.Name))
            throw new ArgumentException("Category name cannot be empty");

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task<ProductCategory?> UpdateAsync(int id, ProductCategory category)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing == null)
            return null;

        existing.Name = category.Name;
        _categoryRepository.Update(existing);
        await _categoryRepository.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing == null)
            return false;

        _categoryRepository.Remove(existing);
        await _categoryRepository.SaveChangesAsync();

        return true;
    }
}