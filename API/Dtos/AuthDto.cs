using System;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class UserLoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserLoginResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

public class RefreshTokenRequestDto
{
    public Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}

public class UserLogoutRequestDto
{
    public required string RefreshToken { get; set; }
}

public class UserLogoutResponseDto
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}