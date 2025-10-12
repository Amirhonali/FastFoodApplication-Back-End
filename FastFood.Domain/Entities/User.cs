using System;

namespace FastFood.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Tg_Id { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<Order> Orders { get; set; }
}
