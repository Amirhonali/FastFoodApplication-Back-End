using Microsoft.EntityFrameworkCore;
using FastFood.Application.DTOs.ReportDTOs;
using FastFood.Application.Interfaces;
using FastFood.Infrastructure.Data;
using FastFood.Domain.Enums;
using FastFood.Domain.Entities;

namespace FastFood.Infrastructure.Services;

public class ReportingService : IReportingService
{
    private readonly AppDbContext _context;

    public ReportingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CashRegister> CloseCashRegisterAsync(CashRegisterCloseDTO dto)
    {
        var date = dto.Date.Date;
        var next = date.AddDays(1);

        // 1️⃣ Проверяем, не закрыта ли касса за этот день
        if (await _context.CashRegisters.AnyAsync(c => c.Date == date))
            throw new Exception("Касса за этот день уже закрыта.");

        // 2️⃣ Подсчитываем доход (все завершённые заказы за день)
        var income = await _context.Orders
            .Where(o => o.Status == OrderStatus.Completed &&
                        o.OrderDate >= date && o.OrderDate < next)
            .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m;

        // 3️⃣ Загружаем все продукты и их ингредиенты за день
        var orders = await _context.Orders
            .Where(o => o.Status == OrderStatus.Completed &&
                        o.OrderDate >= date && o.OrderDate < next)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductIngredients)
                        .ThenInclude(pi => pi.Ingredient)
            .ToListAsync();

        // 4️⃣ Получаем все IngredientArrivals одним запросом
        var ingredientArrivals = await _context.IngredientArrivals
            .ToListAsync();

        // 5️⃣ Считаем себестоимость ингредиентов
        decimal ingredientCost = 0m;

        foreach (var order in orders)
        {
            foreach (var item in order.OrderItems)
            {
                foreach (var pi in item.Product.ProductIngredients)
                {
                    var ingredient = pi.Ingredient;
                    var arrivals = ingredientArrivals
                        .Where(a => a.IngredientId == ingredient.Id)
                        .ToList();

                    var totalQty = arrivals.Sum(a => a.Quantity);
                    var totalPrice = arrivals.Sum(a => a.Quantity * a.UnitPrice);
                    var avgPrice = totalQty > 0 ? totalPrice / totalQty : 0m;

                    ingredientCost += avgPrice * pi.Quantity * item.Quantity;
                }
            }
        }

        // 6️⃣ Считаем чистую прибыль
        var additional = dto.AdditionalExpenses;
        var netProfit = income - (ingredientCost + additional);

        // 7️⃣ Сохраняем запись кассы
        var cashRegister = new CashRegister
        {
            Date = date,
            Income = income,
            IngredientCost = ingredientCost,
            AdditionalExpenses = additional,
            NetProfit = netProfit,
            IsClosed = true
        };

        _context.CashRegisters.Add(cashRegister);
        await _context.SaveChangesAsync();

        return cashRegister;
    }

    public async Task<decimal> GetIncomeAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Completed &&
                        o.OrderDate >= start && o.OrderDate < end)
            .SumAsync(o => (decimal?)o.TotalPrice) ?? 0m;
    }

    public async Task<decimal> GetIngredientCostForPeriod(DateTime from, DateTime to)
    {
        var orders = await _context.Orders
            .Where(o => o.Status == OrderStatus.Completed &&
                        o.OrderDate >= from && o.OrderDate <= to)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductIngredients)
                        .ThenInclude(pi => pi.Ingredient)
            .ToListAsync();

        var ingredientArrivals = await _context.IngredientArrivals.ToListAsync();
        decimal ingredientCost = 0m;

        foreach (var o in orders)
        {
            foreach (var oi in o.OrderItems)
            {
                foreach (var pi in oi.Product.ProductIngredients)
                {
                    var arrivals = ingredientArrivals.Where(a => a.IngredientId == pi.IngredientId).ToList();
                    var totalQty = arrivals.Sum(a => a.Quantity);
                    var totalPrice = arrivals.Sum(a => a.Quantity * a.UnitPrice);
                    var avgPrice = totalQty > 0 ? totalPrice / totalQty : 0m;

                    ingredientCost += avgPrice * pi.Quantity * oi.Quantity;
                }
            }
        }

        return ingredientCost;
    }
}