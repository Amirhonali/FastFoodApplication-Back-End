using System;
using System.ComponentModel.DataAnnotations;
using FastFood.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FastFood.Application.DTOs.ProductDTOs;

public class ProductCreateDTO
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;

    public int ProductCategoryId { get; set; }

    public IFormFile? ImageFile { get; set; }

    public List<ProductIngredientDTO>? Ingredients { get; set; }
}
