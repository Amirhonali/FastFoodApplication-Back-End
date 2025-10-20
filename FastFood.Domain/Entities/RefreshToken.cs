using System;

namespace FastFood.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int StaffId { get; set; }
    public Staff Staff { get; set; }
}