using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class UserLoginHistory
{
    public Guid Id { get; set; }

    [MaxLength(200)]
    public Guid UserId { get; set; }
    public DateTime LoginDatetime { get; set; }
    public DateTime? LogoutDatetime { get; set; }

    [MaxLength(500)]
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
}