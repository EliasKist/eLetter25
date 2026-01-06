// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
//
// namespace eLetter25.Infrastructure.Persistence;
//
// public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
// {
//     public AppDbContext CreateDbContext(string[] args)
//     {
//         var options = new DbContextOptionsBuilder<AppDbContext>();
//
//         var cs = Environment.GetEnvironmentVariable("ELETTER_CONNECTIONSTRING");
//         if (string.IsNullOrWhiteSpace(cs))
//         {
//             throw new InvalidOperationException(
//                 "Missing connection string. Set environment variable 'ELETTER_CONNECTIONSTRING' (or configure it via User Secrets) before running EF Core design-time operations.");
//         }
//
//         options.UseSqlServer(cs);
//
//         return new AppDbContext(options.Options);
//     }
// }