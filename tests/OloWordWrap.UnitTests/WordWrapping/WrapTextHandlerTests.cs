using OloWordWrap.Application.WordWrapping;

namespace OloWordWrap.UnitTests.WordWrapping;

public sealed class WrapTextHandlerTests
{
    private readonly WrapTextHandler _handler = new();

    [Fact]
    public void Handle_WithText_ReturnsWrappedTextAndItsLines()
    {
        var result = _handler.Handle("No utensils please", 11);

        result.Wrapped.Should().Be("No utensils\nplease");
        result.Lines.Should().Equal("No utensils", "please");
    }

    [Fact]
    public void Handle_WithLongWord_HyphenatesEachBreak()
    {
        var result = _handler.Handle("Supercalifragilistic", 6);

        result.Lines.Should().Equal("Super-", "calif-", "ragil-", "istic");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  \t ")]
    public void Handle_WithEmptyOrWhitespaceText_ReturnsNoLines(string text)
    {
        var result = _handler.Handle(text, 10);

        result.Lines.Should().BeEmpty();
        result.Wrapped.Should().BeEmpty();
    }

    [Fact]
    public void Handle_WithBlankLine_KeepsItAsAnEmptyEntry()
    {
        var result = _handler.Handle("a\n\nb", 5);

        result.Lines.Should().Equal("a", "", "b");
    }
}
