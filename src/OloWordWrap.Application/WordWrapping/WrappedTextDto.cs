using System.ComponentModel.DataAnnotations;

namespace OloWordWrap.Application.WordWrapping;

// [Required] marks the properties as required in the OpenAPI document: both are always returned.
public sealed record WrappedTextDto([property: Required] IReadOnlyList<string> Lines, [property: Required] string Wrapped);
