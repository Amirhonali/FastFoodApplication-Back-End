using System;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Domain.Enums;

namespace FastFood.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;

    public OrderService(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.OrderDate = DateTime.Now;
        order.Status = OrderStatus.Pending;
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();
        return order;
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus newStatus)
    {
        var existingOrder = await _orderRepository.GetByIdAsync(id);
        if (existingOrder == null)
            return false;

        existingOrder.Status = newStatus;
        _orderRepository.Update(existingOrder);
        await _orderRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return false;

        _orderRepository.Remove(order);
        await _orderRepository.SaveChangesAsync();
        return true;
    }
}