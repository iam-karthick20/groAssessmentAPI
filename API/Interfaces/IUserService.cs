using API.Dtos;
using API.Entities;

namespace API.Interfaces;

public interface IUserService
{
    Task<UserCreateResponseDto?> UserCreateAsync(UserCreateRequestDto request);
    Task<List<UserGetAllResponseDto?>> UserGetAllAsync();
    Task<UsersGetResponseDto?> UserGetAsync(Guid userId);
    Task<UserUpdateResponseDto?> UserUpdateAsync(UserUpdateRequestDto request);
} 