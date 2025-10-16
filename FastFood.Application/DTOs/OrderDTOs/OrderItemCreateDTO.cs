using System;

namespace FastFood.Application.DTOs.OrderDTOs;

public class OrderItemCreateDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}