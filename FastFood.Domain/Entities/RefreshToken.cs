using System;

namespace FastFood.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public int StaffId { get; set; }
    public Staff Staff { get; set; }

    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public bool IsUsed { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}