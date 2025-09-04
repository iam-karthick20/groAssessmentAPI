namespace API.Exceptions;

public class UserNotFoundException(string username) : Exception($"User with username '{username}' not found.")
{
}

public class UserPasswordInvalidException(string username) : Exception($" User with Username: '{username}' password is invalid.")
{
}

public class RefreshTokenUserNotFoundException() : Exception($"User not found.")
{
}

public class RefreshTokenInvalidException() : Exception($"Refresh Token is invalid.")
{
}

public class RefreshTokenExpiredException() : Exception($"Refresh Token is expired.")
{
}

public class RefreshTokenRovokedException() : Exception($"Refresh Token is revoked.")
{
}

public class LogoutInvalidException() : Exception($"Invalid Logout.")
{
}