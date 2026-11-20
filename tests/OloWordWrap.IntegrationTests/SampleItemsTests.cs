using System.Net.Http.Json;
using OloWordWrap.Application.SampleItems;

namespace OloWordWrap.IntegrationTests;

public sealed class SampleItemsTests(ApiFactory factory)
{
    [Fact]
    public async Task GetSampleItems_ReturnsTheInMemoryItems()
    {
        using var client = factory.CreateClient();

        var items = await client.GetFromJsonAsync<List<SampleItemDto>>(
            new Uri("/api/sample-items", UriKind.Relative), TestContext.Current.CancellationToken);

        items.Should().NotBeNull();
        items!.Select(item => item.Name).Should().Equal("First sample item", "Second sample item", "Third sample item");
    }
}
