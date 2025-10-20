using System;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Staff staff);
}