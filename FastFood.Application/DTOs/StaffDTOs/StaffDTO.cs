using System;

namespace FastFood.Application.DTOs.StaffDTOs;

public class StaffDTO
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public List<string> Permissions { get; set; } = new();
    public string Token { get; set; }
}