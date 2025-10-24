using System;

namespace FastFood.Domain.Entities;

public class CashTransaction
{
    public int Id { get; set; }

    public int ShiftId { get; set; }
    public CashShift Shift { get; set; }

    public decimal Amount { get; set; }
    public string Type { get; set; } // "Income" | "Expense"
    public string Category { get; set; } // "Order", "Salary", "Purchase"
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CreatedById { get; set; }
    public Staff CreatedBy { get; set; }
}