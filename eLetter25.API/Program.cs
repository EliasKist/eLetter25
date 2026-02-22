using eLetter25.API;
using eLetter25.Application;
using eLetter25.Infrastructure;
using eLetter25.Infrastructure.Auth.Data;
using eLetter25.Infrastructure.Persistence;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWeb(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var appScope = app.Services.CreateScope();
    var appDbContext = appScope.ServiceProvider.GetRequiredService<AppDbContext>();
    appDbContext.Database.Migrate();

    using var identityScope = app.Services.CreateScope();
    var identityDbContext = identityScope.ServiceProvider.GetRequiredService<MsIdentityDbContext>();
    identityDbContext.Database.Migrate();

    var roleManager = identityScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
    }
}


app.MapGet("/", () => "eLetter25.API is running...");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();

app.Run();