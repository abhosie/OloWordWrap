using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OloWordWrap.Application.WordWrapping;

namespace OloWordWrap.Api.Controllers;

[ApiController]
[Route("api/word-wrap")]
public sealed class WordWrapController(WrapTextHandler handler) : ControllerBase
{
    // Caps the body before it is read and parsed: 5,000 characters of escaped JSON fits well inside 64 KB,
    // whereas Kestrel's default (30 MB) would let a huge body be parsed before the length check rejects it.
    private const int _maxBodyBytes = 64 * 1024;

    // [ApiController] returns the 400 ValidationProblemDetails before this runs when the request is invalid.
    [HttpPost]
    [RequestSizeLimit(_maxBodyBytes)]
    [ProducesResponseType<WrappedTextDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status413PayloadTooLarge)]
    public ActionResult<WrappedTextDto> Wrap(
        WrapTextRequest request,
        [FromQuery, BindRequired, Range(1, int.MaxValue)] int limit) =>
        Ok(handler.Handle(request.Text, limit));
}
