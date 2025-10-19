using System;

namespace FastFood.Application.DTOs.OrderDTOs;

public class OrderItemResponseDTO
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}