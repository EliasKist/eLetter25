using eLetter25.Application.Auth.Ports;
using eLetter25.Application.Common.Ports;
using eLetter25.Application.Letters.Ports;
using eLetter25.Infrastructure.Auth.Services;
using eLetter25.Infrastructure.DocumentStorage;
using eLetter25.Infrastructure.DomainEvents;
using eLetter25.Infrastructure.Observability;
using eLetter25.Infrastructure.Persistence;
using eLetter25.Infrastructure.Persistence.Letters;
using eLetter25.Infrastructure.Persistence.Letters.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eLetter25.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("eletter25-db")));

        services.AddScoped<IDocumentStorage, LocalFileSystemDocumentStorage>();

        services.AddScoped<ILetterDomainToDbMapper, LetterDomainToDbMapper>();
        services.AddScoped<ILetterDbToDomainMapper, LetterDbToDomainMapper>();
        services.AddScoped<ILetterRepository, EfLetterRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddScoped<IDomainEventCollector, DomainEventCollector>();
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
        services.AddScoped<IAuditWriter, LoggerAuditWriter>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        services.AddScoped<IUserRegistrationService, UserRegistrationService>();
    }
}