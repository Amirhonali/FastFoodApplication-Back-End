using System;
using System.Text.Json.Serialization;
using FastFood.Domain.Enums;

namespace FastFood.Domain.Entities;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IngredientCategory IngredientCategory { get; set; }
    public decimal Quantity { get; set; } 
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }

    [JsonIgnore]
    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
    public ICollection<IngredientArrival> IngredientArrivals { get; set; } = new List<IngredientArrival>();
    public DateTime CreatedAt { get; set; } 
}
