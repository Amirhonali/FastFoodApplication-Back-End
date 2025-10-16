using System;

namespace FastFood.Application.DTOs.ExpenseDTOs;

public class ExpenseDTO
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; } = "Other";
    public DateTime? Date { get; set; }
}