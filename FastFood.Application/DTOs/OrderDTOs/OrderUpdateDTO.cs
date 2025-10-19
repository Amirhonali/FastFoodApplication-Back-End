using System;
using FastFood.Domain.Enums;

namespace FastFood.Application.DTOs.OrderDTOs;

public class OrderUpdateDTO
    {
        public string Location { get; set; }
        public OrderType Type { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
    }