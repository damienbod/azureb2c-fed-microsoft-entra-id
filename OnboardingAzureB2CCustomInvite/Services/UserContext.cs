using Microsoft.EntityFrameworkCore;

namespace OnboardingAzureB2CCustomInvite.Services;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options) { }

    public DbSet<UserEntity> Users { get; set; }
}
