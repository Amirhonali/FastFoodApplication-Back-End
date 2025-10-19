using System;
using FastFood.Domain.Enums;

namespace FastFood.Application.DTOs.OrderDTOs;

public class OrderCreateDTO
{
    public int UserId { get; set; }
    public string Location { get; set; }
    public OrderType Type { get; set; }
    public List<OrderItemDTO> Items { get; set; } = new();
}