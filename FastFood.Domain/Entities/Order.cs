using System;
using FastFood.Domain.Enums;

namespace FastFood.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public decimal TotalPrice { get; set; }
    public string Location { get; set; }
    public OrderType Type { get; set; }
    //public string PaymentMethod { get; set; }
}
