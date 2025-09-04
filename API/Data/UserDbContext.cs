using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserLoginHistory> UsersLoginHistory { get; set; }
    public DbSet<PostEntity> PostEntities { get; set; }
}
