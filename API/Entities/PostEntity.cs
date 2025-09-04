using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class PostEntity
{
    [Key]
    public Guid PostID { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; } = string.Empty;

    public int Liked { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
}
