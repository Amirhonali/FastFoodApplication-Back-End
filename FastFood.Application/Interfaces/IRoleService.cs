using System;
using FastFood.Application.DTOs.RoleDTOs;

namespace FastFood.Application.Interfaces;

public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllAsync();
        Task<RoleDTO?> GetByIdAsync(int id);
        Task<RoleDTO> CreateAsync(RoleCreateDTO dto);
        Task<bool> UpdateAsync(int id, RoleUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }