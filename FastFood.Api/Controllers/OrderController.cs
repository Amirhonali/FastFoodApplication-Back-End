using FastFood.Application.DTOs.OrderDTOs;
using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FastFood.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        // ✅ GET: api/order
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllAsync();

            var response = orders.Select(o => new OrderResponseDTO
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Status = o.Status,
                UserName = o.User?.FullName ?? "Anonymous",
                Location = o.Location,
                Type = o.Type,
                TotalPrice = o.TotalPrice,
                Items = o.OrderItems.Select(i => new OrderItemResponseDTO
                {
                    ProductName = i.Product?.Name ?? "Unknown",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SubTotal = i.SubTotal
                }).ToList()
            });

            return Ok(response);
        }

        // ✅ GET: api/order/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound(new { message = $"Order with id={id} not found" });

            var response = new OrderResponseDTO
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                UserName = order.User?.FullName ?? "Anonymous",
                Location = order.Location,
                Type = order.Type,
                TotalPrice = order.TotalPrice,
                Items = order.OrderItems.Select(i => new OrderItemResponseDTO
                {
                    ProductName = i.Product?.Name ?? "Unknown",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SubTotal = i.SubTotal
                }).ToList()
            };

            return Ok(response);
        }

        // ✅ POST: api/order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = new Order
            {
                UserId = dto.UserId,
                Location = dto.Location,
                Type = dto.Type,
                OrderItems = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var created = await _service.CreateOrderAsync(order);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { created.Id, created.OrderNumber });
        }

        // ✅ PUT: api/order/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedOrder = new Order
            {
                Location = dto.Location,
                Type = dto.Type,
                OrderItems = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            try
            {
                var result = await _service.UpdateOrderAsync(id, updatedOrder);

                if (result == null)
                    return NotFound(new { message = $"Order with id={id} not found" });

                return Ok(new
                {
                    message = "Order updated successfully",
                    totalPrice = result.TotalPrice
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ PATCH: api/order/{id}/status
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] OrderStatus status)
        {
            var success = await _service.UpdateStatusAsync(id, status);
            if (!success)
                return BadRequest(new { message = "Failed to update status" });

            return Ok(new { message = $"Order status updated to {status}" });
        }

        // ✅ POST: api/order/{id}/confirm
        [HttpPost("{id:int}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            await _service.ConfirmOrderAsync(id);
            return Ok(new { message = "Order confirmed successfully" });
        }

        // ✅ POST: api/order/{id}/cancel
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _service.CancelOrderAsync(id);
            return Ok(new { message = "Order cancelled and ingredients restored" });
        }

        // ✅ DELETE: api/order/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Order with id={id} not found" });

            return NoContent();
        }
    }
}