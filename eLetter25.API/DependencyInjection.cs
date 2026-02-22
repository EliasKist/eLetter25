using System.Text;
using eLetter25.Application.Auth.Options;
using eLetter25.Application.Common.Options;
using eLetter25.Infrastructure.Auth.Data;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eLetter25.API;

public static class DependencyInjection
{
    public static void AddWeb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddControllers();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey), "Jwt:SecretKey is missing")
            .ValidateOnStart();

        services.AddOptions<DocumentStorageOptions>()
            .BindConfiguration(DocumentStorageOptions.SectionName)
            .ValidateDataAnnotations()
            .Validate(o => !string.IsNullOrWhiteSpace(o.BasePath), "DocumentStorage:BasePath is missing")
            .ValidateOnStart();

        services.AddDbContext<MsIdentityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("users-db")));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<MsIdentityDbContext>();

        services.AddHangfire((_, config) =>
        {
            var connectionString = configuration.GetConnectionString("hangfire-db")
                                   ?? configuration.GetConnectionString("eletter25-db");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string configuration is missing. Please ensure either " +
                    "\"ConnectionStrings:hangfire-db\" or \"ConnectionStrings:eletter25-db\" is configured.");
            }

            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(o => o.UseNpgsqlConnection(connectionString), new PostgreSqlStorageOptions
                {
                    SchemaName = "hangfire"
                });
        });

        services.AddHangfireServer();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, jwtOpt) =>
            {
                var jwt = jwtOpt.Value;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();
    }
}