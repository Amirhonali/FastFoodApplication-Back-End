using System;

namespace FastFood.Application.DTOs.TokenDTOs;

public class RefreshRequestDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}