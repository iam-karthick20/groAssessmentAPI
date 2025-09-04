using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using API.Exceptions;
using API.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponseDto>> Login(UserLoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }

            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UserPasswordInvalidException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserLoginResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request);
                return Ok(result);
            }
            catch (RefreshTokenUserNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (RefreshTokenInvalidException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (RefreshTokenExpiredException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (RefreshTokenRovokedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            

        }

        [HttpPost("logout")]
        public async Task<ActionResult<UserLogoutResponseDto>> Logout(UserLogoutRequestDto request)
        {
            try
            {
                var result = await _authService.LogoutAsync(request);
                return Ok(result);
            }
            catch (RefreshTokenRovokedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (LogoutInvalidException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("Authenticated user");
        }
 
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndPoint()
        {
            return Ok("Authenticated user");
        }
    }
}
