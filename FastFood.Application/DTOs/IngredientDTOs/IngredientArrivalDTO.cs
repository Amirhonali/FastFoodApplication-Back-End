using System;
using System.ComponentModel.DataAnnotations;

namespace FastFood.Application.DTOs.IngredientDTOs;

 public class IngredientArrivalDTO
    {
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public DateTime ArrivalDate { get; set; } = DateTime.Now;
    }