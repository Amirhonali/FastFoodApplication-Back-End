using System;

namespace FastFood.Application.DTOs.StaffDTOs;

public class StaffCreateDTO
    {
        public string FullName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public int? BranchId { get; set; }
    }