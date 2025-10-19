using System;
using FastFood.Application.DTOs.OrderDTOs;
using FastFood.Domain.Entities;
using FastFood.Domain.Enums;

namespace FastFood.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order?> UpdateOrderAsync(int id, Order updatedOrder);
    Task<bool> DeleteAsync(int id);

    Task<bool> UpdateStatusAsync(int orderId, OrderStatus newStatus);
    Task<bool> CancelOrderAsync(int orderId);
    Task<bool> ConfirmOrderAsync(int orderId);

    Task<bool> RecalculateIngredientsAsync(int orderId);
}