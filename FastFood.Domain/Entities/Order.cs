using System;
using FastFood.Domain.Enums;

namespace FastFood.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public OrderSource Source { get; set; } = OrderSource.Customer;
    public OrderType Type { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public int? StaffId { get; set; }
    public Staff? Staff { get; set; }

    
    public int BranchId { get; set; }
    public Branch Branch { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public decimal TotalPrice { get; set; }

    public string? TableNumber { get; set; } 
    public string? Location { get; set; }   
}