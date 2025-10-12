using System;
using FastFood.Domain.Entities;
using FastFood.Domain.Enums;

namespace FastFood.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(int id);
    Task<Order> CreateOrderAsync(Order order);
    Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus);
    Task<bool> DeleteOrderAsync(int id);
}