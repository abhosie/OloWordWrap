using Microsoft.Extensions.DependencyInjection;
using OloWordWrap.Application.SampleItems;

namespace OloWordWrap.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetSampleItemsHandler>();

        return services;
    }
}
