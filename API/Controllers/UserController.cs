using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using API.Exceptions;
using API.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService _userService) : ControllerBase
    {
        [HttpPost("")]
        public async Task<ActionResult<UserCreateResponseDto>> UserCreate(UserCreateRequestDto request)
        {
            try
            {
                var user = await _userService.UserCreateAsync(request);

                return StatusCode(StatusCodes.Status201Created, user);
            }

            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DuplicateUserException ex)
            {
                return Conflict(new { message = ex.Message }); // HTTP 409
            }
            catch (DuplicateUserEmailException ex)
            {
                return Conflict(new { message = ex.Message }); // HTTP 409
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<ActionResult<List<UsersGetResponseDto>>> UserGetAll()
        {
            try
            {
                var users = await _userService.UserGetAllAsync();

                return Ok(users);
            }
            catch (UsersGetNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<List<UsersGetResponseDto>>> UserGet(Guid userId)
        {
            try
            {
                var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimUserId) || claimUserId != userId.ToString())
                {
                    return Forbid(); 
                }

                var users = await _userService.UserGetAsync(userId);

                return Ok(users);
            }
            catch (UserGetNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpPatch("")]
        public async Task<ActionResult<UserUpdateResponseDto>> UserUpdate(UserUpdateRequestDto request)
        {
            try
            {
                var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(claimUserId) || claimUserId != request.UserId.ToString())
                {
                    return Forbid(); 
                }

                var user = await _userService.UserUpdateAsync(request);
                return Ok(user);
            }
            catch (DuplicateUserException ex)
            {
                return Conflict(new { message = ex.Message }); // HTTP 409
            }
            catch (DuplicateUserEmailException ex)
            {
                return Conflict(new { message = ex.Message }); // HTTP 409
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }


}
