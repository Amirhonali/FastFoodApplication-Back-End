using System;

namespace FastFood.Application.DTOs.PermissionDTOs;

public class PermissionCreateDTO
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}