using System;

namespace API.Exceptions;

public class PostNotFoundException(Guid postID) : Exception($"Post with Post ID: '{postID}' not found.")
{
}

public class UserPostNotFoundException(Guid userId) : Exception($"User Post with User ID: '{userId}' not found.")
{
}