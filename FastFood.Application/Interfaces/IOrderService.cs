using System;
using FastFood.Application.DTOs.OrderDTOs;
using FastFood.Domain.Entities;
using FastFood.Domain.Enums;

namespace FastFood.Application.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(OrderCreateDTO dto);
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<bool> CancelOrderAsync(int id);
    Task<bool> CompleteOrderAsync(int id); // confirms and decrements ingredients / creates expense records
}