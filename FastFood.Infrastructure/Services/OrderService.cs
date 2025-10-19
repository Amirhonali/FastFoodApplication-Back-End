using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Domain.Enums;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        //Получить все заказы
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .AsNoTracking()
                .ToListAsync();
        }

        //Получить заказ по ID
        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        //Создание нового заказа
        public async Task<Order> CreateOrderAsync(Order order)
        {
            // Уменьшаем количество ингредиентов при создании
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products
                    .Include(p => p.ProductIngredients)
                        .ThenInclude(pi => pi.Ingredient)
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                    throw new Exception($"Product with ID={item.ProductId} not found");

                foreach (var pi in product.ProductIngredients)
                {
                    pi.Ingredient.Quantity -= pi.Quantity * item.Quantity;
                    if (pi.Ingredient.Quantity < 0)
                        throw new Exception($"Not enough {pi.Ingredient.Name} in stock");
                }

                item.UnitPrice = product.Price;
            }

            order.TotalPrice = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);
            order.Status = OrderStatus.Pending;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        //Обновление заказа
        public async Task<Order?> UpdateOrderAsync(int id, Order updatedOrder)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled)
                throw new Exception("Cannot modify completed or cancelled order");

            // если заказ уже подтверждён, пересчитаем ингредиенты
            if (order.Status == OrderStatus.Processing)
                await RecalculateIngredientsAsync(order.Id);

            // очищаем старые позиции
            order.OrderItems.Clear();

            decimal total = 0m;

            // создаём новые позиции и сразу считаем цену
            foreach (var item in updatedOrder.OrderItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                if (product == null)
                    throw new Exception($"Product with id={item.ProductId} not found");

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                total += orderItem.SubTotal;
                order.OrderItems.Add(orderItem);
            }

            order.TotalPrice = total;

            await _context.SaveChangesAsync();
            return order;
        }

        //Удалить заказ
        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        //Обновление статуса заказа
        public async Task<bool> UpdateStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) throw new Exception("Order not found");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Cannot update a cancelled order");

            if (newStatus == OrderStatus.Cancelled)
                return await CancelOrderAsync(orderId);

            order.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        //Подтверждение заказа (Pending → Processing)
        public async Task<bool> ConfirmOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Order cannot be confirmed");

            order.Status = OrderStatus.Processing;
            await _context.SaveChangesAsync();

            return true;
        }

        //Отмена заказа (возврат ингредиентов)
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductIngredients)
                            .ThenInclude(pi => pi.Ingredient)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Order already cancelled");

            // возвращаем ингредиенты
            foreach (var item in order.OrderItems)
            {
                foreach (var pi in item.Product.ProductIngredients)
                {
                    pi.Ingredient.Quantity += pi.Quantity * item.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }

        //Пересчёт ингредиентов (при изменении заказа)
        public async Task<bool> RecalculateIngredientsAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductIngredients)
                            .ThenInclude(pi => pi.Ingredient)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            foreach (var item in order.OrderItems)
            {
                foreach (var pi in item.Product.ProductIngredients)
                {
                    pi.Ingredient.Quantity -= pi.Quantity * item.Quantity;
                    if (pi.Ingredient.Quantity < 0)
                        throw new Exception($"Not enough {pi.Ingredient.Name} in stock");
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order> CreateOrderFromCartAsync(string userId, Cart cart, string location)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
                throw new Exception("User not found");

            var order = new Order
            {
                UserId = user.Id,
                Location = location,
                Status = OrderStatus.Pending,
                OrderItems = cart.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList(),
                TotalPrice = cart.TotalPrice
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}