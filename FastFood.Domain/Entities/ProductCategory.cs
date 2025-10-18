using System;
using System.Text.Json.Serialization;

namespace FastFood.Domain.Entities;

public class ProductCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [JsonIgnore]
    public ICollection<Product>? Products { get; set; }
}
