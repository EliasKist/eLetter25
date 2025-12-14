using eLetter25.Infrastructure.Persistence.Letters;
using Microsoft.EntityFrameworkCore;

namespace eLetter25.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<LetterDbEntity> Letters { get; set; } = null!;
    public DbSet<LetterTagDbEntity> LetterTags { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    }
}