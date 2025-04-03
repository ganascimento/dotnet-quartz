using Dotnet.Quartz.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Quartz.Api.Context;

public class AppDataContext : DbContext
{
    public DbSet<UserEntity>? User { get; set; }

    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
    { }
}