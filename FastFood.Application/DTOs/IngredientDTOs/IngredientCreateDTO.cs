using System;
using System.ComponentModel.DataAnnotations;
using FastFood.Domain.Enums;

namespace FastFood.Application.DTOs.IngredientDTOs;

public class IngredientCreateDTO
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public IngredientCategory IngredientCategory { get; set; }

    public decimal Quantity { get; set; } = 0m;
    public decimal? Weight { get; set; }
    public decimal? Volume { get; set; }
}
