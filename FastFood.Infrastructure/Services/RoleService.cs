using System;
using FastFood.Application.DTOs.RoleDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Services;

public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;
        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleDTO>> GetAllAsync()
        {
            return await _context.Roles
                .Include(r => r.Permissions)
                .Select(r => new RoleDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Permissions = r.Permissions.Select(p => p.Code).ToList()
                })
                .ToListAsync();
        }

        public async Task<RoleDTO?> GetByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return null;

            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role.Permissions.Select(p => p.Code).ToList()
            };
        }

        public async Task<RoleDTO> CreateAsync(RoleCreateDTO dto)
        {
            var permissions = await _context.Permissions
                .Where(p => dto.PermissionIds.Contains(p.Id))
                .ToListAsync();

            var role = new Role
            {
                Name = dto.Name,
                Permissions = permissions
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role.Permissions.Select(p => p.Code).ToList()
            };
        }

        public async Task<bool> UpdateAsync(int id, RoleUpdateDTO dto)
        {
            var role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return false;

            var newPermissions = await _context.Permissions
                .Where(p => dto.PermissionIds.Contains(p.Id))
                .ToListAsync();

            role.Name = dto.Name;
            role.Permissions = newPermissions;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }