using System;
using API.Interfaces;
using API.Services;

namespace API.DependencyInjection;

public static class ScopedServices
{
public static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IPostService, PostService>();

        
        return services;
    }
}
