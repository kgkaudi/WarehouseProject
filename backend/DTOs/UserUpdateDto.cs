using System;

namespace backend.DTOs;

public class UserUpdateDto
{
    public string Username { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}