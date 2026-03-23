using System;

namespace backend.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Dimensions { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public double Weight { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}