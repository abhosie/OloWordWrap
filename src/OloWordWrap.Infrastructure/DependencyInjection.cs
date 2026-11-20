using Microsoft.Extensions.DependencyInjection;

namespace OloWordWrap.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// No database: nothing is stored. External services (and their options) are registered here when a
    /// feature needs one.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services;
    }
}
