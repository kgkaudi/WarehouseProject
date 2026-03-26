using System;

namespace backend.DTOs;

public class UserReadDto
{
    public string Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<ProductReadDto> Products { get; set; } = new();
}