using System;

namespace backend.DTOs;

public class UserCreateDto
{
    public string Username { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
}

