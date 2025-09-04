using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class User
{
    public Guid UserId { get; set; }

    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Role { get; set; } = string.Empty;

    [MaxLength(100)]
    public string EmailAddress { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }

    [MaxLength(2000)]
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
}
