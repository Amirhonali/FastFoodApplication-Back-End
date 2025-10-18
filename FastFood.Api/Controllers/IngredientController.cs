using FastFood.Application.DTOs.IngredientDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _service;

        public IngredientController(IIngredientService service)
        {
            _service = service;
        }

        // ✅ GET: api/ingredient
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ingredients = await _service.GetAllAsync();
            return Ok(ingredients);
        }

        // ✅ GET: api/ingredient/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ingredient = await _service.GetByIdAsync(id);
            if (ingredient == null)
                return NotFound(new { message = $"Ingredient with id={id} not found" });

            return Ok(ingredient);
        }

        // ✅ POST: api/ingredient
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IngredientCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ingredient = new Ingredient
            {
                Name = dto.Name,
                IngredientCategory = dto.IngredientCategory,
                Quantity = dto.Quantity,
                Weight = dto.Weight,
                Volume = dto.Volume
            };

            var created = await _service.CreateAsync(ingredient);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ✅ PUT: api/ingredient/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] IngredientUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ingredient = new Ingredient
            {
                Name = dto.Name,
                IngredientCategory = dto.IngredientCategory,
                Quantity = dto.Quantity,
                Weight = dto.Weight,
                Volume = dto.Volume
            };

            var updated = await _service.UpdateAsync(id, ingredient);
            if (updated == null)
                return NotFound(new { message = $"Ingredient with id={id} not found" });

            return Ok(updated);
        }

        // ✅ DELETE: api/ingredient/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = $"Ingredient with id={id} not found" });

            return NoContent();
        }

        // ✅ GET: api/ingredient/search?name=sugar
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var result = await _service.SearchAsync(name);
            return Ok(result);
        }

        [HttpPost("{id:int}/arrival")]
        public async Task<IActionResult> AddArrival(int id, [FromBody] IngredientArrivalDTO dto)
        {
            var arrival = new IngredientArrival
            {
                IngredientId = id,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                ArrivalDate = dto.ArrivalDate
            };

            try
            {
                var created = await _service.AddArrivalAsync(arrival);

                // ✅ После сохранения подгружаем данные ингредиента
                created = await _service.GetArrivalByIdAsync(created.Id);

                var response = new IngredientArrivalResponseDTO
                {
                    Id = created.Id,
                    Quantity = created.Quantity,
                    UnitPrice = created.UnitPrice,
                    ArrivalDate = created.ArrivalDate,
                    IngredientName = created.Ingredient?.Name
                };

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ✅ GET: api/ingredient/{id}/arrivals
        [HttpGet("{id:int}/arrivals")]
        public async Task<IActionResult> GetArrivals(int id)
        {
            var arrivals = await _service.GetArrivalsAsync(id);
            return Ok(arrivals);
        }
    }
}