using FastFood.Application.DTOs.StaffDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastFood.Infrastructure.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public StaffService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<StaffDTO> LoginAsync(LoginDTO dto)
        {
            var staff = await _context.Staffs
                .Include(s => s.Role)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(s => s.Username == dto.Username);

            if (staff == null || !BCrypt.Net.BCrypt.Verify(dto.Password, staff.PasswordHash))
                throw new Exception("Неверный логин или пароль");

            return new StaffDTO
            {
                Id = staff.Id,
                FullName = staff.FullName,
                Username = staff.Username,
                Role = staff.Role.Name,
                Permissions = staff.Role.Permissions.Select(p => p.Code).ToList(),
                Token = GenerateJwtToken(staff)
            };
        }

        public async Task<StaffDTO> CreateAsync(StaffDTO dto, string password)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role);
            if (role == null) throw new Exception("Role not found");

            var staff = new Staff
            {
                FullName = dto.FullName,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RoleId = role.Id,
                IsActive = true
            };

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            return new StaffDTO
            {
                Id = staff.Id,
                FullName = staff.FullName,
                Username = staff.Username,
                Role = role.Name,
                Permissions = role.Permissions.Select(p => p.Code).ToList()
            };
        }

        public async Task<IEnumerable<StaffDTO>> GetAllAsync()
        {
            return await _context.Staffs
                .Include(s => s.Role)
                .Select(s => new StaffDTO
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Username = s.Username,
                    Role = s.Role.Name
                })
                .ToListAsync();
        }

        private string GenerateJwtToken(Staff staff)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("id", staff.Id.ToString()),
                new Claim("username", staff.Username),
                new Claim("role", staff.Role.Name)
            };

            foreach (var perm in staff.Role.Permissions)
                claims.Add(new Claim("permission", perm.Code));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}