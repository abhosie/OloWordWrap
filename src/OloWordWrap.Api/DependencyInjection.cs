using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace OloWordWrap.Api;

internal static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddProblemDetails();
        // Infrastructure adds its own checks (e.g. the database) to the same /health endpoint.
        services.AddHealthChecks();
        services.AddOpenApi();
        services.AddRateLimiting(configuration);

        return services;
    }

    /// <summary>
    /// A global fixed-window limit per client IP address, from the "RateLimiting" configuration section.
    /// Behind a reverse proxy, configure forwarded headers first or every client shares the proxy's address.
    /// </summary>
    private static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var settings = context.RequestServices.GetRequiredService<IOptions<RateLimitingOptions>>().Value;
                return RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.PermitLimit,
                        Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                    });
            });
        });

        return services;
    }
}
