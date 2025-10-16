using System;
using FastFood.Application.DTOs.ProductDTOs;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IProductIngredientService
{
    Task AddOrUpdateIngredientsAsync(int productId, List<ProductIngredientDTO> items);
    Task<IEnumerable<ProductIngredient>> GetByProductIdAsync(int productId);
}