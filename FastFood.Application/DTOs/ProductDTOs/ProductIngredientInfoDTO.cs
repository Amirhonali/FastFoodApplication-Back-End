using System;

namespace FastFood.Application.DTOs.ProductDTOs;

public class ProductIngredientInfoDTO
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}