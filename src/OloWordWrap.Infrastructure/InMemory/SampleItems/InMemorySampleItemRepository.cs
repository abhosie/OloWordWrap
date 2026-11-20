using OloWordWrap.Application.SampleItems;
using OloWordWrap.Domain.SampleItems;

namespace OloWordWrap.Infrastructure.InMemory.SampleItems;

/// <summary>
/// Placeholder storage for a project without a database: three fixed items, so the placeholder endpoint
/// has something to return. Delete it with the rest of the SampleItem feature.
/// </summary>
internal sealed class InMemorySampleItemRepository : ISampleItemRepository
{
    private static readonly IReadOnlyList<SampleItem> _items =
    [
        new(Guid.Parse("0195a1b2-0000-7000-8000-000000000001"), "First sample item"),
        new(Guid.Parse("0195a1b2-0000-7000-8000-000000000002"), "Second sample item"),
        new(Guid.Parse("0195a1b2-0000-7000-8000-000000000003"), "Third sample item"),
    ];

    public Task<IReadOnlyList<SampleItem>> ListAsync(CancellationToken cancellationToken) => Task.FromResult(_items);
}
