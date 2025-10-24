using FastFood.Application.DTOs.StaffDTOs;
using FastFood.Application.DTOs.TokenDTOs;
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

            if (staff == null || !BCrypt.Net.BCrypt.Verify(dto.Password, staff.PasswordHash))
                throw new Exception("Неверный логин или пароль");

            if (!staff.IsActive)
                throw new Exception("Пользователь заблокирован");

            var accessToken = _jwtService.GenerateToken(staff);
            var refreshToken = await GenerateRefreshTokenAsync(staff.Id);

            _logger.LogInformation("Пользователь {Username} успешно вошёл", dto.Username);

            return new LoginResultDTO
            {
                Id = staff.Id,
                FullName = staff.FullName,
                Username = staff.Username,
                Role = staff.Role.Name,
                Permissions = staff.Role.Permissions.Select(p => p.Code).ToList(),
                Token = accessToken,
                RefreshToken = refreshToken
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

        private async Task<string> GenerateRefreshTokenAsync(int staffId)
        {
            var token = new RefreshToken
            {
                StaffId = staffId,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
            return token.Token;
        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(RefreshRequestDTO dto)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal == null)
                throw new Exception("Неверный access токен");

            var staffId = int.Parse(principal.FindFirst("id").Value);

            var refresh = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken && r.StaffId == staffId);

            if (refresh == null || refresh.IsUsed || refresh.IsRevoked || refresh.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Неверный или просроченный refresh токен");

            refresh.IsUsed = true;
            await _context.SaveChangesAsync();

            var staff = await _context.Staffs
                .Include(s => s.Role)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(s => s.Id == staffId);

            var newAccessToken = _jwtService.GenerateToken(staff);
            var newRefreshToken = await GenerateRefreshTokenAsync(staff.Id);

            return new TokenResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(12)
            };
        }
    }
}