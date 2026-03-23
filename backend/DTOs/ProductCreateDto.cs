using System;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public class ProductCreateDto
{
    [Required]
    [MinLength(2)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public string Dimensions { get; set; } = string.Empty;

    public double Price { get; set; }
    public int Quantity { get; set; }
    [Range(0.01, double.MaxValue)]
    public double Weight { get; set; }
}