using FastFood.Application.DTOs.StaffDTOs;
using FastFood.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly ILogger<StaffController> _logger;

        public StaffController(IStaffService staffService, ILogger<StaffController> logger)
        {
            _staffService = staffService;
            _logger = logger;
        }

        /// <summary>
        /// Авторизация сотрудника (официант, кассир, повар и т.д.)
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _staffService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Ошибка входа: {Message}", ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Создание нового сотрудника (только для администратора)
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] StaffCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var staff = await _staffService.CreateAsync(dto);
                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании сотрудника");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Получить всех сотрудников (доступно для администратора)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var staffList = await _staffService.GetAllAsync();
                return Ok(staffList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка сотрудников");
                return StatusCode(500, new { message = "Ошибка сервера" });
            }
        }
    }
}