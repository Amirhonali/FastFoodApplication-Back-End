using System;

namespace FastFood.Application.DTOs.PermissionDTOs;

public class PermissionDTO
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}