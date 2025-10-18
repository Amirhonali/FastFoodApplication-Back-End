using System;

namespace FastFood.Application.DTOs.IngredientDTOs;

public class IngredientArrivalResponseDTO
{
    public int Id { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime ArrivalDate { get; set; }
    public string IngredientName { get; set; }
}