using System;
using FastFood.Application.DTOs.PermissionDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Services;

public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _context;
        public PermissionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermissionDTO>> GetAllAsync()
        {
            return await _context.Permissions
                .Select(p => new PermissionDTO
                {
                    Id = p.Id,
                    Code = p.Code,
                    Description = p.Description
                })
                .ToListAsync();
        }

        public async Task<PermissionDTO?> GetByIdAsync(int id)
        {
            var p = await _context.Permissions.FindAsync(id);
            if (p == null) return null;

            return new PermissionDTO
            {
                Id = p.Id,
                Code = p.Code,
                Description = p.Description
            };
        }

        public async Task<PermissionDTO> CreateAsync(PermissionCreateDTO dto)
        {
            var permission = new Permission
            {
                Code = dto.Code,
                Description = dto.Description
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return new PermissionDTO
            {
                Id = permission.Id,
                Code = permission.Code,
                Description = permission.Description
            };
        }

        public async Task<bool> UpdateAsync(int id, PermissionUpdateDTO dto)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return false;

            permission.Description = dto.Description;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return false;

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return true;
        }
    }