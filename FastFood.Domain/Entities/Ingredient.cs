using System;
using FastFood.Domain.Enums;

namespace FastFood.Domain.Entities;

public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IngredientCategory IngredientCategory { get; set; }
        public int? Quantity { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
