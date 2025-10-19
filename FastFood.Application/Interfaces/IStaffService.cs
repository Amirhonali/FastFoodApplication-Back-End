using System;
using FastFood.Application.DTOs.StaffDTOs;

namespace FastFood.Application.Interfaces;

public interface IStaffService
    {
        Task<StaffDTO> LoginAsync(LoginDTO dto);
        Task<StaffDTO> CreateAsync(StaffDTO dto, string password);
        Task<IEnumerable<StaffDTO>> GetAllAsync();
    }