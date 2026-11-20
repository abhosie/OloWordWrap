using Microsoft.Extensions.DependencyInjection;
using OloWordWrap.Application.WordWrapping;

namespace OloWordWrap.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<WrapTextHandler>();

        return services;
    }
}
