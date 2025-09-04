using System;
using Microsoft.EntityFrameworkCore;
using API.Dtos;
using API.Exceptions;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using API.Data;
using API.Entities;

namespace API.Services;

public class UserService(UserDbContext _context) : IUserService
{
    public async Task<UserCreateResponseDto?> UserCreateAsync(UserCreateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Username and password are required.");

        // Check if username already exists
        var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == request.Username);
        if (existingUser is not null)
            throw new DuplicateUserException(request.Username);

        // Check if Email already exists
        var existingUserEmail = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);
        if (existingUserEmail is not null)
            throw new DuplicateUserEmailException(request.EmailAddress);

        // Create new user
        var user = new User
        {
            Username = request.Username,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role, 
            EmailAddress = request.EmailAddress,
            IsActive = true,
            CreatedOn = DateTime.Now,
        };

        // Secure password hashing
        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);


        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserCreateResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.Role,
            EmailAddress = user.EmailAddress,
            CreatedOn = user.CreatedOn
        };
    }

    public async Task<List<UserGetAllResponseDto?>> UserGetAllAsync()
    {
        var user = await _context.Users.AsNoTracking().Select(u => new UserGetAllResponseDto
        {
            UserId = u.UserId.ToString(),
            Username = u.Username!,
            EmailAddress = u.EmailAddress!,
            Role = u.Role!,
            IsActive = u.IsActive,
            CreatedOn = u.CreatedOn,
            ModifiedBy = u.ModifiedBy,
            ModifiedOn = u.ModifiedOn

        }).ToListAsync() ?? throw new UsersGetNotFoundException();

        return user!;
    }

    // User Get by Guid UserID
    public async Task<UsersGetResponseDto?> UserGetAsync(Guid userId)
    {
        var user = await _context.Users
        .AsNoTracking()
        .Where(u => u.UserId == userId)
        .Select(u => new UsersGetResponseDto
        {
            UserId = u.UserId,
            Username = u.Username!,
            EmailAddress = u.EmailAddress!,
            Role = u.Role!,
            IsActive = u.IsActive,
            CreatedOn = u.CreatedOn,
            ModifiedBy = u.ModifiedBy,
            ModifiedOn = u.ModifiedOn
        })
        .FirstOrDefaultAsync() ?? throw new UserGetNotFoundException(userId);

        return user; 
    }
    
    // User Update
    public async Task<UserUpdateResponseDto?> UserUpdateAsync(UserUpdateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Username and password are required.");

        // Load user to update
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId) ?? throw new UpdateUserNotFoundException(request.UserId.ToString().ToUpper());

        // Check if username already exists
        var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == request.Username && u.UserId != request.UserId);
        if (existingUser is not null)
            throw new DuplicateUserException(request.Username);

        // Check if username already exists
        var existingUserEmail = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress && u.UserId != request.UserId);
        if (existingUserEmail is not null)
            throw new DuplicateUserEmailException(request.EmailAddress);

        user.Username = request.Username;
        user.EmailAddress = request.EmailAddress;
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.ModifiedBy = request.ModifiedBy;
        user.ModifiedOn = DateTime.Now;

        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await _context.SaveChangesAsync();

        return new UserUpdateResponseDto
        {
            Username = request.Username,
            Message = "Updated Successfully"
        };
    }
}
