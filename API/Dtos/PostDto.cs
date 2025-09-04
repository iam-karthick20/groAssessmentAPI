using System;

namespace API.Dtos;

public class CreatePostRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IFormFile ImageFile { get; set; } = default!;
    public Guid CreatedBy { get; set; }
}

public class ResponseDto
{
    public bool Sucess { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class GetPostResponseDto
{
    public Guid PostID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int Liked { get; set; }
}

public class PostDashboardResponseDto
{
    public string Title { get; set; } = string.Empty;
    public int Liked { get; set; }
}