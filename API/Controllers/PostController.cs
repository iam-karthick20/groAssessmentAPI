using System.Security.Claims;
using API.Dtos;
using API.Exceptions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController(IPostService _service) : ControllerBase
    {
        [Authorize]
        [HttpPost("")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostRequestDto request)
        {
            var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            request.CreatedBy = Guid.Parse(claimUserId!);

            var post = await _service.CreatePostAsync(request);

            return StatusCode(201, post);
        }

        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<List<GetPostResponseDto>>> GetAllPosts()
        {
            var posts = await _service.GetAllPostsAsync();
            return Ok(posts);
        }

        [Authorize]
        [HttpPost("like/{postId}")]
        public async Task<IActionResult> LikePost(Guid postId)
        {
            try
            {
                var response = await _service.LikePostAsync(postId);
                return Ok(response);
            }
            catch (PostNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }
        
        [Authorize]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var userID = Guid.Parse(claimUserId!);

                var response = await _service.PostDashboardAsync(userID);
                return Ok(response);
            }
            catch (UserPostNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }

    }
}
