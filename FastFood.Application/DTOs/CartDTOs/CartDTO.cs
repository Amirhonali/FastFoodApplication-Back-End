using System;

namespace FastFood.Application.DTOs.CartDTOs;

public class CartDTO
    {
        public int UserId { get; set; }
        public List<CartItemDTO> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(i => i.Total);
    }