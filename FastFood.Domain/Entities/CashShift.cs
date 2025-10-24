using System;

namespace FastFood.Domain.Entities;

public class CashShift
{
    public int Id { get; set; }

    public int CashierId { get; set; }
    public Staff Cashier { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; }

    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public decimal OpeningAmount { get; set; }
    public decimal ClosingAmount { get; set; }

    public bool IsOpen { get; set; } = true;

    public ICollection<CashTransaction> Transactions { get; set; } = new List<CashTransaction>();
}