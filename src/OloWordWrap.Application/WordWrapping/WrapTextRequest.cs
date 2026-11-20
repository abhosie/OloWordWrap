using System.ComponentModel.DataAnnotations;

namespace OloWordWrap.Application.WordWrapping;

// Validation attributes sit on the constructor parameter: MVC reads record validation from there.
// AllowEmptyStrings stops MVC adding an implicit [Required] that would reject "" and whitespace.
public sealed record WrapTextRequest(
    [Required(AllowEmptyStrings = true), StringLength(WrapTextRequest.TextMaxLength)] string Text)
{
    public const int TextMaxLength = 5000;
}
