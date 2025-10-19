using System;

namespace FastFood.Application.DTOs.CartDTOs;


public class CartItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }