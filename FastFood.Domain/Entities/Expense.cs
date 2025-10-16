using System;

namespace FastFood.Domain.Entities;

public class Expense
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Category { get; set; } 
}