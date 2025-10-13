using System;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IOrderItemService
    {
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<OrderItem?> GetByIdAsync(int id);
        Task AddAsync(OrderItem orderItem);
        Task<OrderItem?> UpdateAsync(int id, OrderItem orderItem);
        Task<bool> DeleteAsync(int id);
    }
