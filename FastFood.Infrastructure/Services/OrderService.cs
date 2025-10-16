using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastFood.Application.DTOs.OrderDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(OrderCreateDTO dto)
    {
        var order = new Order
        {
            UserId = dto.UserId,
            Location = dto.Location,
            OrderDate = DateTime.Now,
            Status = FastFood.Domain.Enums.OrderStatus.Pending
        };

        decimal total = 0m;
        foreach (var item in dto.Items)
        {
            var prod = await _context.Products.FindAsync(item.ProductId);
            if (prod == null) throw new KeyNotFoundException($"Product {item.ProductId} not found");

            order.OrderItems.Add(new OrderItem
            {
                ProductId = prod.Id,
                Quantity = item.Quantity,
                UnitPrice = prod.Price
            });

            total += prod.Price * item.Quantity;
        }

        order.TotalPrice = total;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductIngredients)
                        .ThenInclude(pi => pi.Ingredient)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> CancelOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;
        if (order.Status == FastFood.Domain.Enums.OrderStatus.Completed) return false;

        order.Status = FastFood.Domain.Enums.OrderStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }

    // Complete: check inventory, subtract ingredient quantities, compute ingredient cost
    public async Task<bool> CompleteOrderAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductIngredients)
                        .ThenInclude(pi => pi.Ingredient)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return false;
        if (order.Status == FastFood.Domain.Enums.OrderStatus.Completed) return false;

        // check inventory
        foreach (var oi in order.OrderItems)
        {
            var product = oi.Product;
            foreach (var pi in product.ProductIngredients)
            {
                var required = pi.Quantity * oi.Quantity;
                var ing = pi.Ingredient;
                var current = ing.Quantity;
                if (current < required)
                    throw new InvalidOperationException($"Ingredient {ing.Name} not enough (required {required}, available {current}).");
            }
        }

        // subtract and compute ingredient cost (average unit price)
        decimal ingredientCostTotal = 0m;
        foreach (var oi in order.OrderItems)
        {
            var product = oi.Product;
            foreach (var pi in product.ProductIngredients)
            {
                var required = pi.Quantity * oi.Quantity;
                var ing = pi.Ingredient;

                // reduce
                var current = ing.Quantity;
                ing.Quantity = current - required;

                // average unit price from arrivals
                var arrivals = await _context.IngredientArrivals
                    .Where(a => a.IngredientId == ing.Id)
                    .ToListAsync();

                decimal unitPrice = 0m;
                var totalQty = arrivals.Sum(a => a.Quantity);
                var totalPrice = arrivals.Sum(a => a.Quantity * a.UnitPrice);
                if (totalQty > 0)
                    unitPrice = totalPrice / totalQty;

                ingredientCostTotal += unitPrice * required;
            }
        }

        order.Status = FastFood.Domain.Enums.OrderStatus.Completed;
        // optionally: store ingredient cost somewhere — e.g., as an Expense record or in order metadata
        await _context.SaveChangesAsync();
        return true;
    }
}