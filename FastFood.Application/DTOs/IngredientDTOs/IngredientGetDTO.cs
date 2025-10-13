using System;

namespace FastFood.Application.DTOs.IngredientDTOs;

public class IngredientGetDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Quantity { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
}
