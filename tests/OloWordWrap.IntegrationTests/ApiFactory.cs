using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using OloWordWrap.Api;

[assembly: AssemblyFixture(typeof(OloWordWrap.IntegrationTests.ApiFactory))]

namespace OloWordWrap.IntegrationTests;

/// <summary>
/// Runs the real API in memory, shared by every test in the run. No database: nothing to start or seed.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // The in-memory test server has no client IP, so every test shares one rate-limit partition.
        builder.UseSetting($"{RateLimitingOptions.SectionName}:{nameof(RateLimitingOptions.PermitLimit)}", "100000");
    }
}
