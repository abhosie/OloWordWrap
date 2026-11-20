using System.ComponentModel.DataAnnotations;

namespace OloWordWrap.Application.SampleItems;

// [Required] marks the properties as required in the OpenAPI document, so the generated
// TypeScript types aren't optional.
public sealed record SampleItemDto([property: Required] Guid Id, [property: Required] string Name);
