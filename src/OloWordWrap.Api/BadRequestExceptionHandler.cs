using Microsoft.AspNetCore.Diagnostics;

namespace OloWordWrap.Api;

/// <summary>
/// Kestrel throws <see cref="BadHttpRequestException"/> while the body is read (too large, timed out, malformed).
/// Without this, the exception handler turns it into a 500 and logs it as an unhandled error.
/// </summary>
public sealed class BadRequestExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badRequest)
        {
            return false;
        }

        httpContext.Response.StatusCode = badRequest.StatusCode;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
        });
    }
}
