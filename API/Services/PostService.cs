using System;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Exceptions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class PostService(UserDbContext _context) : IPostService
{
    public async Task<ResponseDto> CreatePostAsync(CreatePostRequestDto request)
    {
        var newPostId = Guid.NewGuid();

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var fileName = $"{newPostId}{Path.GetExtension(request.ImageFile.FileName)}";

        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.ImageFile.CopyToAsync(stream);
        }

        var post = new PostEntity
        {
            PostID = newPostId,
            Title = request.Title,
            Description = request.Description,
            CreatedBy = request.CreatedBy,
            CreatedOn = DateTime.Now,
            ImageUrl = $"/uploads/{fileName}",
        };

        _context.PostEntities.Add(post);
        await _context.SaveChangesAsync();

        return new ResponseDto
        {
            Sucess = true,
            Message = "Posted Sucessfully"
        };
    }

    public async Task<List<GetPostResponseDto>> GetAllPostsAsync()
    {
        var posts = await _context.PostEntities
          .OrderByDescending(p => p.CreatedOn)
          .Select(p => new GetPostResponseDto
          {
              PostID = p.PostID,
              Title = p.Title,
              Description = p.Description,
              ImageUrl = p.ImageUrl!,
              CreatedBy = p.CreatedBy,
              CreatedOn = p.CreatedOn,
              Liked = p.Liked
          })
          .ToListAsync();

        return posts;
    }

    public async Task<ResponseDto> LikePostAsync(Guid PostID)
    {
        var post = await _context.PostEntities.FirstOrDefaultAsync(u => u.PostID == PostID) ?? throw new PostNotFoundException(PostID);

        post.Liked += 1;
        await _context.SaveChangesAsync();

        return new ResponseDto
        {
            Sucess = true,
            Message = "Post Liked Successfully"
        };
    }

    public async Task<List<PostDashboardResponseDto>> PostDashboardAsync(Guid UserId)
    {
        var post = await _context.PostEntities.Where(u => u.CreatedBy == UserId).ToListAsync() ?? throw new UserPostNotFoundException(UserId);

        var response = post.Select(p => new PostDashboardResponseDto
        {
            Title = p.Title,
            Liked = p.Liked
        }).ToList();

        return response;
    }

}
