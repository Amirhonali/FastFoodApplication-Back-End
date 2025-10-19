using System;

namespace FastFood.Domain.Entities;
public class Staff
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }  // логин в системе
    public string PasswordHash { get; set; }
    public string? PhoneNumber { get; set; }

    public int RoleId { get; set; }
    public Role Role { get; set; }

    public bool IsActive { get; set; } = true;

    // Привязка к филиалу или рабочей зоне
    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }
}