using System;
using FastFood.Application.DTOs.StaffDTOs;

namespace FastFood.Application.Interfaces;

public interface IStaffService
{
    Task<LoginResultDTO> LoginAsync(LoginDTO dto);
    Task<StaffResponseDTO> CreateAsync(StaffCreateDTO dto);
    Task<IEnumerable<StaffResponseDTO>> GetAllAsync();
}