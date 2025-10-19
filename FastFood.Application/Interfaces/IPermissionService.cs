using System;
using FastFood.Application.DTOs.PermissionDTOs;

namespace FastFood.Application.Interfaces;

 public interface IPermissionService
    {
        Task<IEnumerable<PermissionDTO>> GetAllAsync();
        Task<PermissionDTO?> GetByIdAsync(int id);
        Task<PermissionDTO> CreateAsync(PermissionCreateDTO dto);
        Task<bool> UpdateAsync(int id, PermissionUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }