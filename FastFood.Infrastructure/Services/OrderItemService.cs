using System;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;

namespace FastFood.Infrastructure.Services;

public class OrderItemService : IOrderItemService
{
    private readonly IRepository<OrderItem> _orderItemRepository;

    public OrderItemService(IRepository<OrderItem> orderItemRepository)
    {
        _orderItemRepository = orderItemRepository;
    }

    public async Task<IEnumerable<OrderItem>> GetAllAsync()
    {
        return await _orderItemRepository.GetAllAsync();
    }

    public async Task<OrderItem?> GetByIdAsync(int id)
    {
        return await _orderItemRepository.GetByIdAsync(id);
    }

    public async Task AddAsync(OrderItem orderItem)
    {
        if (orderItem.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (orderItem.UnitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero");

        await _orderItemRepository.AddAsync(orderItem);
        await _orderItemRepository.SaveChangesAsync();
    }

    public async Task<OrderItem?> UpdateAsync(int id, OrderItem orderItem)
    {
        var existing = await _orderItemRepository.GetByIdAsync(id);
        if (existing == null)
            return null;

        existing.ProductId = orderItem.ProductId;
        existing.Quantity = orderItem.Quantity;
        existing.UnitPrice = orderItem.UnitPrice;
        existing.OrderId = orderItem.OrderId;

        _orderItemRepository.Update(existing);
        await _orderItemRepository.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _orderItemRepository.GetByIdAsync(id);
        if (existing == null)
            return false;

        _orderItemRepository.Remove(existing);
        await _orderItemRepository.SaveChangesAsync();

        return true;
    }
}