using System;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.DependencyInjection;

public static class DbContextServices
{
    public static IServiceCollection AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("UserDatabase")));
            
        return services;
    }
}