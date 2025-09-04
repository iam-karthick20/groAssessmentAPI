using System;
using API.Dtos;

namespace API.Interfaces;

public interface IAuthService
{
    Task<UserLoginResponseDto?> LoginAsync(UserLoginRequestDto request);
    Task<UserLoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<UserLogoutResponseDto?> LogoutAsync(UserLogoutRequestDto request); 
}
