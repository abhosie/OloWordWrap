using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OloWordWrap.Api;

namespace OloWordWrap.IntegrationTests;

public sealed class BadRequestExceptionHandlerTests
{
    [Theory]
    [InlineData(StatusCodes.Status413PayloadTooLarge)]
    [InlineData(StatusCodes.Status400BadRequest)]
    [InlineData(StatusCodes.Status408RequestTimeout)]
    public async Task TryHandleAsync_WithBadHttpRequest_WritesItsOwnStatusAsProblemDetails(int statusCode)
    {
        // The in-memory test server doesn't enforce Kestrel limits, so the handler is called directly.
        var services = new ServiceCollection().AddLogging().AddProblemDetails().BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = services };
        context.Response.Body = new MemoryStream();
        var handler = new BadRequestExceptionHandler(services.GetRequiredService<IProblemDetailsService>());

        var handled = await handler.TryHandleAsync(
            context, new BadHttpRequestException("Request body too large.", statusCode), TestContext.Current.CancellationToken);

        handled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(statusCode);
        context.Response.Body.Position = 0;
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body, JsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(statusCode); // asserted non-null on the line above
    }

    [Fact]
    public async Task TryHandleAsync_WithOtherException_DoesNotHandleIt()
    {
        var services = new ServiceCollection().AddLogging().AddProblemDetails().BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = services };
        var handler = new BadRequestExceptionHandler(services.GetRequiredService<IProblemDetailsService>());

        var handled = await handler.TryHandleAsync(context, new InvalidOperationException("boom"), TestContext.Current.CancellationToken);

        handled.Should().BeFalse();
    }
}
