using FastFood.Application.DTOs.CartDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FastFood.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly IMemoryCache _cache;
        private readonly AppDbContext _context;
        private readonly string _cachePrefix = "cart_";

        public CartService(IMemoryCache cache, AppDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        private string GetCacheKey(int userId) => $"{_cachePrefix}{userId}";

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            var key = GetCacheKey(userId);
            var cart = _cache.Get<CartDTO>(key) ?? new CartDTO { UserId = userId };

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId)
                ?? throw new Exception("Продукт не найден");

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem is not null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItemDTO
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = quantity,
                    Price = product.Price
                });
            }

            _cache.Set(key, cart, TimeSpan.FromHours(2));
        }

        public Task RemoveFromCartAsync(int userId, int productId)
        {
            var key = GetCacheKey(userId);
            var cart = _cache.Get<CartDTO>(key);
            if (cart is null) return Task.CompletedTask;

            cart.Items.RemoveAll(i => i.ProductId == productId);
            _cache.Set(key, cart, TimeSpan.FromHours(2));

            return Task.CompletedTask;
        }

        public Task<CartDTO> GetCartAsync(int userId)
        {
            var cart = _cache.Get<CartDTO>(GetCacheKey(userId)) ?? new CartDTO { UserId = userId };
            return Task.FromResult(cart);
        }

        public Task ClearCartAsync(int userId)
        {
            _cache.Remove(GetCacheKey(userId));
            return Task.CompletedTask;
        }

        public async Task CheckoutAsync(int userId)
        {
            var key = GetCacheKey(userId);
            var cart = _cache.Get<CartDTO>(key) ?? throw new Exception("Корзина пуста");

            if (!cart.Items.Any())
                throw new Exception("Корзина пуста");

            var order = new Order
            {
                UserId = userId,
                TotalPrice = cart.TotalPrice,
                Location = "Default Location",  
                OrderItems = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _cache.Remove(key);
        }
    }
}