using System;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User?> GetByTgIdAsync(string tgId);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
    }