using System;
using FastFood.Domain.Enums;

namespace FastFood.Application.DTOs.IngredientDTOs;

public class IngredientDTO
{
    public string Name { get; set; } = string.Empty;
    public IngredientCategory IngredientCategory { get; set; }
    public decimal? Quantity { get; set; } // ombordagi mavjud miqdor (normalizatsiya qilindi decimal)
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
}
