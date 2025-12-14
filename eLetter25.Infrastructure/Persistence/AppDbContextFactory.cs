using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eLetter25.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();

        var cs = Environment.GetEnvironmentVariable("ELETTER_CONNECTIONSTRING") ??
                 "Server=localhost,1433;Database=eletter25db;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False;";

        options.UseSqlServer(cs);

        return new AppDbContext(options.Options);
    }
}