using FastFood.Domain.Entities;
using FastFood.Domain.Enums;

namespace FastFood.Application.Interfaces
{
    public interface IOrderService
    {
        //Основные операции
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(int id, Order updatedOrder);
        Task<bool> DeleteAsync(int id);

        //Управление статусами
        Task<bool> UpdateStatusAsync(int orderId, OrderStatus newStatus);
        Task<bool> CancelOrderAsync(int orderId);
        Task<bool> ConfirmOrderAsync(int orderId);

        //Пересчёт ингредиентов
        Task<bool> RecalculateIngredientsAsync(int orderId);
        Task<Order> CreateOrderFromCartAsync(string userId, Cart cart, string location);
    }
}