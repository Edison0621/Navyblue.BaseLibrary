using Microsoft.EntityFrameworkCore;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;
using NavyblueWebApi.Infrastructure.Persistence.Configurations;

namespace NavyblueWebApi.Infrastructure.Persistence;

/// <summary>
///     EF Core <see cref="DbContext" /> for the Navyblue Web API template (MySQL 8.0 via Pomelo).
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => this.Set<User>();

    public DbSet<Auth> Auths => this.Set<Auth>();

    public DbSet<RefreshToken> RefreshTokens => this.Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AuthConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
