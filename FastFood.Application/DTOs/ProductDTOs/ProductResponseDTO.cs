using System;

namespace FastFood.Application.DTOs.ProductDTOs;

public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public List<ProductIngredientInfoDTO> Ingredients { get; set; } = new();
    }