using System;
using FastFood.Application.DTOs.CartDTOs;

namespace FastFood.Application.Interfaces;

public interface ICartService
    {
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task RemoveFromCartAsync(int userId, int productId);
        Task<CartDTO> GetCartAsync(int userId);
        Task ClearCartAsync(int userId);
        Task CheckoutAsync(int userId); // создаёт заказ на основе корзины
    }