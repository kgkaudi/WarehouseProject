using System;

namespace backend.DTOs;

public class UserReadDto
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required string CompanyName { get; set; }
    public required string CompanyAddress { get; set; }
    public required string Role { get; set; }
    public required List<ProductReadDto> Products { get; set; }
}