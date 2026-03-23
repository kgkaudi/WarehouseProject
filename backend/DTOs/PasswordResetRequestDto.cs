using System;

namespace backend.DTOs;

public class PasswordResetRequestDto
{
    public string Email { get; set; } = string.Empty;
}
