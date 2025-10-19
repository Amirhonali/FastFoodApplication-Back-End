using System;

namespace FastFood.Application.DTOs.RoleDTOs;

public class RoleDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
}