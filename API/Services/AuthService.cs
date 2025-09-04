using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Dtos;
using API.Exceptions;
using API.Interfaces;
using API.Entities;
using API.Data;

namespace API.Services;

public class AuthService(UserDbContext _context, IConfiguration _configuration) : IAuthService
{
    #region Login with JWT Token

    public async Task<UserLoginResponseDto?> LoginAsync(UserLoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Username and password are required.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username) ?? throw new UserNotFoundException(request.Username);

        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash!, request.Password) == PasswordVerificationResult.Failed)
            throw new UserPasswordInvalidException(request.Username);

        return await CreateTokenResponse(user);
    }

    private async Task<UserLoginResponseDto> CreateTokenResponse(User user)
    {
        var refreshToken = GenerateRefreshToken();

        var userLogin = new UserLoginHistory
        {
            UserId = user.UserId,
            LoginDatetime = DateTime.Now,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddDays(_configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationDays"))
        };

        _context.UsersLoginHistory.Add(userLogin);
        await _context.SaveChangesAsync();

        return new UserLoginResponseDto
        {
            AccessToken = CreateToken(user),
            RefreshToken = refreshToken
        };
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username!),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Role, user.Role!),
            new(ClaimTypes.Email, user.EmailAddress!),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JwtSettings:SecretKey")!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("JwtSettings:Issuer"),
            audience: _configuration.GetValue<string>("JwtSettings:Audience"),
            claims: claims,
            expires: DateTime.Now.AddDays(_configuration.GetValue<int>("JwtSettings:ExpirationDays")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    #region Refresh Token 

    public async Task<UserLoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

        return await CreateTokenResponse(user!);
    }

    public async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
    {
        var user = await _context.UsersLoginHistory.FirstOrDefaultAsync(u => u.UserId == userId && u.RefreshToken == refreshToken && u.LogoutDatetime == null) ?? throw new RefreshTokenUserNotFoundException();

        if (user.RefreshToken != refreshToken)
            throw new RefreshTokenInvalidException();

        if (user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            user.IsRevoked = true;
            user.RevokedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            throw new RefreshTokenExpiredException();
        }

        if(user.IsRevoked is true)
            throw new RefreshTokenRovokedException();

        user.IsRevoked = true;
        user.RevokedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        var userDetails = await _context.Users.FindAsync(userId) ?? throw new RefreshTokenUserNotFoundException();

        return userDetails;
    }

    #endregion

    #endregion

    // Logout
    public async Task<UserLogoutResponseDto?> LogoutAsync(UserLogoutRequestDto request)
    {
        var user = await _context.UsersLoginHistory.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && u.LogoutDatetime == null) ?? throw new LogoutInvalidException();

        if (user.IsRevoked is true)
            throw new RefreshTokenRovokedException();
            
        user.LogoutDatetime = DateTime.Now;
        user.IsRevoked = true;
        user.RevokedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return new UserLogoutResponseDto
        {
            Message = "Logout Successfully"
        };
    }
}






