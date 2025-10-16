using System;

namespace FastFood.Application.DTOs.ProductDTOs;

public class ProductIngredientGetDTO
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; }
    public decimal Quantity { get; set; }
}
