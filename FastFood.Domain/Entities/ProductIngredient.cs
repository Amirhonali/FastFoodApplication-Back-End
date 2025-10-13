using System;

namespace FastFood.Domain.Entities;

public class ProductIngredient
{
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }

    public decimal Quantity { get; set; } 
}