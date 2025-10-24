using System;
using System.Security.Claims;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Staff staff);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

}