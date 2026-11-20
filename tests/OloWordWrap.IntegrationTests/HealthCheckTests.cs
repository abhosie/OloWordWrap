using System.Net;

namespace OloWordWrap.IntegrationTests;

public sealed class HealthCheckTests(ApiFactory factory)
{
    [Fact]
    public async Task GetHealth_Returns200()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/health", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
