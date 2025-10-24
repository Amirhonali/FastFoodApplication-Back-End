using System;

namespace FastFood.Application.DTOs.TokenDTOs;

public class TokenResponseDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}