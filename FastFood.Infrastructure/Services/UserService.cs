using System;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;

namespace FastFood.Infrastructure.Services;

public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByTgIdAsync(string tgId)
        {
            var users = await _userRepository.FindAsync(u => u.Tg_Id == tgId);
            return users.FirstOrDefault();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existing = await _userRepository.GetByIdAsync(user.Id);
            if (existing == null) return false;

            existing.FullName = user.FullName;
            existing.PhoneNumber = user.PhoneNumber;
            existing.Address = user.Address;

            _userRepository.Update(existing);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }
    }