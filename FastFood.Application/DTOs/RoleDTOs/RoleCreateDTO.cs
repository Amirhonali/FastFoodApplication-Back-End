using System;

namespace FastFood.Application.DTOs.RoleDTOs;

public class RoleCreateDTO
{
    public string Name { get; set; } = string.Empty;
    public List<int> PermissionIds { get; set; } = new();
}