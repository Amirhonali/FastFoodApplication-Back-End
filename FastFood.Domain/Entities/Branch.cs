using System;

namespace FastFood.Domain.Entities;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }

    public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<CashShift> CashShifts { get; set; } = new List<CashShift>();
}