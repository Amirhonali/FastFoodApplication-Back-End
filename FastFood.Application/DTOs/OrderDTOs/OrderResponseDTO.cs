using System;
using FastFood.Domain.Enums;

namespace FastFood.Application.DTOs.OrderDTOs;

public class OrderResponseDTO
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }

    public string UserName { get; set; }
    public string Location { get; set; }
    public OrderType Type { get; set; }

    public decimal TotalPrice { get; set; }

    public List<OrderItemResponseDTO> Items { get; set; } = new();
}