namespace OloWordWrap.Domain.SampleItems;

/// <summary>
/// Placeholder entity that shows every layer end to end (table, endpoint, page, tests).
/// Rename it into the first real feature, or delete it: see "Placeholder feature" in the README.
/// </summary>
public sealed class SampleItem(Guid id, string name)
{
    public const int NameMaxLength = 200;

    public Guid Id { get; private set; } = id;

    public string Name { get; private set; } = name;
}
