using System;

namespace FastFood.Domain.Entities;

public class Permission
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }