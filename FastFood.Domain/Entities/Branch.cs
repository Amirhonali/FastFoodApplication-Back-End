using System;

namespace FastFood.Domain.Entities;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }

    public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
}