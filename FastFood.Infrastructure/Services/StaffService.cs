using FastFood.Application.DTOs.StaffDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FastFood.Infrastructure.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _context;
        private readonly IJwtTokenService _jwtService;
        private readonly ILogger<StaffService> _logger;

        public StaffService(AppDbContext context, IJwtTokenService jwtService, ILogger<StaffService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginResultDTO> LoginAsync(LoginDTO dto)
        {
            var staff = await _context.Staffs
                .Include(s => s.Role)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(s => s.Username == dto.Username);

            if (staff == null)
            {
                _logger.LogWarning("Неудачная попытка входа: пользователь {Username} не найден", dto.Username);
                throw new Exception("Неверный логин или пароль");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, staff.PasswordHash))
            {
                _logger.LogWarning("Неудачная попытка входа для {Username}: неверный пароль", dto.Username);
                throw new Exception("Неверный логин или пароль");
            }

            if (!staff.IsActive)
                throw new Exception("Пользователь заблокирован");

            _logger.LogInformation("Пользователь {Username} успешно вошёл", dto.Username);

            return new LoginResultDTO
            {
                Id = staff.Id,
                FullName = staff.FullName,
                Username = staff.Username,
                Role = staff.Role.Name,
                Permissions = staff.Role.Permissions.Select(p => p.Code).ToList(),
                Token = _jwtService.GenerateToken(staff)
            };
        }

        public async Task<StaffResponseDTO> CreateAsync(StaffCreateDTO dto)
        {
            if (await _context.Staffs.AnyAsync(s => s.Username == dto.Username))
                throw new Exception("Пользователь с таким логином уже существует");

            var role = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Name == dto.Role)
                ?? throw new Exception("Роль не найдена");

            var staff = new Staff
            {
                FullName = dto.FullName,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = role.Id,
                PhoneNumber = dto.PhoneNumber,
                BranchId = dto.BranchId,
                IsActive = true
            };

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Создан новый сотрудник {Username} с ролью {Role}", dto.Username, dto.Role);

            return new StaffResponseDTO
            {
                Id = staff.Id,
                FullName = staff.FullName,
                Username = staff.Username,
                Role = role.Name,
                IsActive = staff.IsActive
            };
        }

        public async Task<IEnumerable<StaffResponseDTO>> GetAllAsync()
        {
            var result = await _context.Staffs
                .Include(s => s.Role)
                .Select(s => new StaffResponseDTO
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    Username = s.Username,
                    Role = s.Role.Name,
                    IsActive = s.IsActive
                })
                .ToListAsync();

            _logger.LogInformation("Получен список всех сотрудников (кол-во: {Count})", result.Count);
            return result;
        }
    }
}