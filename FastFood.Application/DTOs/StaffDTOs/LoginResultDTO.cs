using System;

namespace FastFood.Application.DTOs.StaffDTOs;

public class LoginResultDTO
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public List<string> Permissions { get; set; } = new();
    public string Token { get; set; } = null!;
}