using System;

namespace FastFood.Domain.Entities;

public class IngredientArrival
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }

    public decimal Quantity { get; set; }  
    public decimal UnitPrice { get; set; } 
    public DateTime ArrivalDate { get; set; } = DateTime.Now;

    public decimal TotalPrice => Quantity * UnitPrice;
}