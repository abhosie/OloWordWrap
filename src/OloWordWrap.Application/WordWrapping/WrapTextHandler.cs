using System.Diagnostics.CodeAnalysis;
using OloWordWrap.Domain.WordWrapping;

namespace OloWordWrap.Application.WordWrapping;

/// <summary>Use case: wrap the special instructions and return them as lines and as one string.</summary>
public sealed class WrapTextHandler
{
    [SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "The controller is given the handler through its constructor, so the use case can gain " +
            "dependencies (e.g. a logger or options) without changing its callers.")]
    public WrappedTextDto Handle(string text, int limit)
    {
        var wrapped = WordWrapper.WordWrap(text, limit);
        IReadOnlyList<string> lines = wrapped.Length == 0 ? [] : wrapped.Split('\n');

        return new WrappedTextDto(lines, wrapped);
    }
}
