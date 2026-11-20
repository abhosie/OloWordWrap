using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OloWordWrap.Api.Controllers;
using OloWordWrap.Application.WordWrapping;

namespace OloWordWrap.IntegrationTests;

public sealed class WordWrapTests(ApiFactory factory)
{
    private const string _brief = "No utensils please, and cook the steak medium rare";

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private static Uri Url(string query) => new($"/api/word-wrap{query}", UriKind.Relative);

    private static StringContent Json(string json) => new(json, Encoding.UTF8, "application/json");

    private async Task<HttpResponseMessage> PostTextAsync(string query, string text)
    {
        using var client = factory.CreateClient();
        return await client.PostAsJsonAsync(Url(query), new { text }, Ct);
    }

    private async Task<HttpResponseMessage> PostRawAsync(string query, string? json)
    {
        using var client = factory.CreateClient();
        using var content = json is null ? new StringContent("", Encoding.UTF8, "application/json") : Json(json);
        return await client.PostAsync(Url(query), content, Ct);
    }

    private static async Task AssertValidationErrorAsync(HttpResponseMessage response, string? field)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json"); // asserted non-null on the line above
        if (field is null)
        {
            return;
        }

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(Ct));
        var keys = body.RootElement.GetProperty("errors").EnumerateObject().Select(property => property.Name);
        keys.Should().Contain(key => key.Contains(field, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task PostWordWrap_WithBriefSentence_ReturnsLinesAndWrapped()
    {
        using var response = await PostTextAsync("?limit=13", _brief);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<WrappedTextDto>(Ct);
        result.Should().NotBeNull(); // the "!" below is safe once this passes
        result!.Lines.Should().Equal("No utensils", "please, and", "cook the", "steak medium", "rare");
        result.Wrapped.Should().Be("No utensils\nplease, and\ncook the\nsteak medium\nrare");
    }

    [Fact]
    public async Task PostWordWrap_WithLineBreaks_KeepsBlankLine()
    {
        using var response = await PostTextAsync("?limit=20", "No utensils\r\n\r\nExtra napkins");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<WrappedTextDto>(Ct);
        result.Should().NotBeNull(); // the "!" below is safe once this passes
        result!.Lines.Should().Equal("No utensils", "", "Extra napkins");
        result.Wrapped.Should().Be("No utensils\n\nExtra napkins");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  \t ")]
    public async Task PostWordWrap_WithEmptyOrWhitespaceText_ReturnsNoLines(string text)
    {
        using var response = await PostTextAsync("?limit=10", text);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<WrappedTextDto>(Ct);
        result.Should().NotBeNull(); // the "!" below is safe once this passes
        result!.Lines.Should().BeEmpty();
        result.Wrapped.Should().BeEmpty();
    }

    [Fact]
    public async Task PostWordWrap_WithTextOfMaxLength_Returns200()
    {
        using var response = await PostTextAsync("?limit=10", new string('a', WrapTextRequest.TextMaxLength));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostWordWrap_WithTextOverMaxLength_Returns400()
    {
        using var response = await PostTextAsync("?limit=10", new string('a', WrapTextRequest.TextMaxLength + 1));

        await AssertValidationErrorAsync(response, "text");
    }

    [Fact]
    public void WrapAction_HasRequestSizeLimit()
    {
        // The in-memory test server does not enforce body size limits (only Kestrel does, and it
        // answers 413), so an over-limit request can't be sent here; check the guard is on the action.
        var action = typeof(WordWrapController).GetMethod(nameof(WordWrapController.Wrap));

        action.Should().NotBeNull();
        action!.GetCustomAttributes(typeof(RequestSizeLimitAttribute), inherit: false).Should().ContainSingle();
    }

    [Fact]
    public async Task PostWordWrap_WithoutLimit_Returns400()
    {
        using var response = await PostTextAsync("", _brief);

        await AssertValidationErrorAsync(response, "limit");
    }

    [Fact]
    public async Task PostWordWrap_WithNonIntegerLimit_Returns400()
    {
        using var response = await PostTextAsync("?limit=abc", _brief);

        await AssertValidationErrorAsync(response, "limit");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task PostWordWrap_WithLimitBelowOne_Returns400(int limit)
    {
        using var response = await PostTextAsync($"?limit={limit}", _brief);

        await AssertValidationErrorAsync(response, "limit");
    }

    [Theory]
    [InlineData("{\"text\":null}")]
    [InlineData("{}")]
    public async Task PostWordWrap_WithNullOrMissingText_Returns400(string json)
    {
        using var response = await PostRawAsync("?limit=10", json);

        await AssertValidationErrorAsync(response, "text");
    }

    [Fact]
    public async Task PostWordWrap_WithNoBody_Returns400()
    {
        using var response = await PostRawAsync("?limit=10", null);

        await AssertValidationErrorAsync(response, null);
    }
}
