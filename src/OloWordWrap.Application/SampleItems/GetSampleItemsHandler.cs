namespace OloWordWrap.Application.SampleItems;

/// <summary>Use case: list every sample item, ordered by name.</summary>
public sealed class GetSampleItemsHandler(ISampleItemRepository repository)
{
    public async Task<IReadOnlyList<SampleItemDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var items = await repository.ListAsync(cancellationToken);

        return items
            .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .Select(item => new SampleItemDto(item.Id, item.Name))
            .ToList();
    }
}
