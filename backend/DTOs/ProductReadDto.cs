namespace backend.DTOs;

public class ProductReadDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Dimensions { get; set; }
    public required double Price { get; set; }
    public required int Quantity { get; set; }
    public required double Weight { get; set; }
}