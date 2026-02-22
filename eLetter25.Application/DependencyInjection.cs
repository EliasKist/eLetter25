using eLetter25.Application.Letters.UseCases.CreateLetter;
using Microsoft.Extensions.DependencyInjection;

namespace eLetter25.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateLetterHandler).Assembly));
    }
}