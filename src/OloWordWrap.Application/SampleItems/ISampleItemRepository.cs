using OloWordWrap.Domain.SampleItems;

namespace OloWordWrap.Application.SampleItems;

/// <summary>What the application needs from storage; implemented in Infrastructure.</summary>
public interface ISampleItemRepository
{
    Task<IReadOnlyList<SampleItem>> ListAsync(CancellationToken cancellationToken);
}
