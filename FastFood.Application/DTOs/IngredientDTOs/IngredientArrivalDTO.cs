using System;

namespace FastFood.Application.DTOs.IngredientDTOs;

public class IngredientArrivalDTO
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime? ArrivalDate { get; set; }
}