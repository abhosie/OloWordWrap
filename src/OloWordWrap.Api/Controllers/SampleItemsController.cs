using Microsoft.AspNetCore.Mvc;
using OloWordWrap.Application.SampleItems;

namespace OloWordWrap.Api.Controllers;

/// <summary>Placeholder endpoint for the starter page. Rename or delete it with the rest of the sample feature.</summary>
[ApiController]
[Route("api/sample-items")]
public sealed class SampleItemsController(GetSampleItemsHandler handler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SampleItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SampleItemDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await handler.HandleAsync(cancellationToken));
}
