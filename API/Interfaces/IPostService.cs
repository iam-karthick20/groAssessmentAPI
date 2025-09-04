using System;
using API.Dtos;

namespace API.Interfaces;

public interface IPostService
{
    Task<ResponseDto> CreatePostAsync(CreatePostRequestDto request);
    Task<List<GetPostResponseDto>> GetAllPostsAsync();
    Task<ResponseDto> LikePostAsync(Guid PostID);
    Task<List<PostDashboardResponseDto>> PostDashboardAsync(Guid UserId);
}
