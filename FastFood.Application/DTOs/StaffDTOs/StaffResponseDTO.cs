using System;

namespace FastFood.Application.DTOs.StaffDTOs;

public class StaffResponseDTO
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool IsActive { get; set; }
}

