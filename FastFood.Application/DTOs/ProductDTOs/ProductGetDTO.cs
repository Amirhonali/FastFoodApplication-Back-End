using System;

namespace FastFood.Application.DTOs.ProductDTOs;

public class ProductGetDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public int ProductCategoryId { get; set; }
    public string? ProductCategoryName { get; set; }
    public List<ProductIngredientGetDTO>? Ingredients { get; set; }
}
