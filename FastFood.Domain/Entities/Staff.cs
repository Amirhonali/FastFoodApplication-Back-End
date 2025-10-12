using System;

namespace FastFood.Domain.Entities;

public class Staff
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; }
}