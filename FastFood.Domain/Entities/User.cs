using System;

namespace FastFood.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string? TelegramId { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }

    // Связь с заказами
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}