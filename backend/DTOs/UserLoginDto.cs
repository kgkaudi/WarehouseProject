using System;

namespace backend.DTOs;

public class UserLoginDto
{
    public string Identifier { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
