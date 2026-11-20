using Microsoft.Extensions.DependencyInjection;
using OloWordWrap.Application.SampleItems;
using OloWordWrap.Infrastructure.InMemory.SampleItems;

namespace OloWordWrap.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// No database: nothing is stored. External services (and their options) are registered here when a
    /// feature needs one.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISampleItemRepository, InMemorySampleItemRepository>();

        return services;
    }
}
