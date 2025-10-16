using System;

namespace FastFood.Domain.Entities;

public class CashRegister
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now.Date;
    public decimal Income { get; set; }
    public decimal IngredientCost { get; set; }
    public decimal AdditionalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public bool IsClosed { get; set; }
}