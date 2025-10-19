using System;

namespace FastFood.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(i => i.SubTotal);
}