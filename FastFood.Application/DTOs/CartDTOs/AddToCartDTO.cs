using System;

namespace FastFood.Application.DTOs.CartDTOs;

public class AddToCartDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}