using System;
using FastFood.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FastFood.Application.DTOs.ProductDTOs;

public class ProductDTO
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }

    // 📷 Foydalanuvchi yuklaydigan rasm
    public IFormFile? Image { get; set; }

    public int ProductCategoryId { get; set; }
    public List<int> IngredientIds { get; set; } = new List<int>();
}
