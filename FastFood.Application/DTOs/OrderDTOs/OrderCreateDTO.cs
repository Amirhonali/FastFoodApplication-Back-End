using System;

namespace FastFood.Application.DTOs.OrderDTOs;

public class OrderCreateDTO
{
    public int UserId { get; set; } // yoki telegram id mapping
    public string Location { get; set; } = string.Empty;
    public List<OrderItemCreateDTO> Items { get; set; } = new();
}