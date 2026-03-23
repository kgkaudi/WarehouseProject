using System;

namespace backend.DTOs;

public class ProductReadDto
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Dimensions { get; set; } = string.Empty;
    public double Price { get; set; }
    public double Weight { get; set; }
}