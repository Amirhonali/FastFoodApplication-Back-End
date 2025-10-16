using System;

namespace FastFood.Domain.Entities;

public class Product : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; }
    public ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();

}
