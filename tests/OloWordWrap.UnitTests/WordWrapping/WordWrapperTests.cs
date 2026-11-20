using OloWordWrap.Domain.WordWrapping;

namespace OloWordWrap.UnitTests.WordWrapping;

public sealed class WordWrapperTests
{
    private const string _brief = "No utensils please, and cook the steak medium rare";

    public static TheoryData<string, int, string> Cases => new()
    {
        // T1-T3: the brief sentence
        { _brief, 10, "No\nutensils\nplease,\nand cook\nthe steak\nmedium\nrare" },
        { _brief, 13, "No utensils\nplease, and\ncook the\nsteak medium\nrare" },
        { _brief, 20, "No utensils please,\nand cook the steak\nmedium rare" },
        // T4: limit 1
        { "a b", 1, "a\nb" },
        { "ab", 1, "a\nb" },
        // T5, T6: exact fit
        { "steak", 5, "steak" },
        { "a b", 3, "a b" },
        // T7: limit larger than the text
        { "No utensils", 100, "No utensils" },
        { "No utensils", int.MaxValue, "No utensils" },
        // T8: one over the fit
        { "No utensils", 11, "No utensils" },
        { "No utensils", 10, "No\nutensils" },
        // T9-T11: long words are split with a hyphen at each break
        { "Supercalifragilistic", 6, "Super-\ncalif-\nragil-\nistic" },
        { "Supercalifragilistic", 5, "Supe-\nrcal-\nifra-\ngili-\nstic" },
        { "a Supercalifragilistic b", 6, "a\nSuper-\ncalif-\nragil-\nistic\nb" },
        // T41: a piece already ending in a hyphen gets no second one
        { "well-done steak", 6, "well-\ndone\nsteak" },
        { "medium-rare steak", 8, "medium-\nrare\nsteak" },
        // a hyphen exactly at the limit is the break: no added hyphen, and the next line doesn't start with one
        { "abcde-fgh", 6, "abcde-\nfgh" },
        // T42: limit 1 has no room for a hyphen (see T4); limit 2 gives one character plus a hyphen
        { "abcd", 2, "a-\nb-\ncd" },
        // T12: a hyphen is not a break point when the word fits
        { "well-done", 20, "well-done" },
        // T13, T14: whitespace collapse
        { "  No   utensils  please  ", 20, "No utensils please" },
        { "No\tutensils \t please", 20, "No utensils please" },
        // T15, T16: empty and whitespace-only
        { "", 10, "" },
        { "   ", 10, "" },
        { "\t \t", 10, "" },
        { " \n \r\n ", 10, "" },
        // T17: typed line breaks
        { "No utensils\nExtra napkins", 20, "No utensils\nExtra napkins" },
        { "No utensils\nExtra napkins", 11, "No utensils\nExtra\nnapkins" },
        // T18: blank lines
        { "No utensils\n\nExtra napkins", 20, "No utensils\n\nExtra napkins" },
        { "a\n   \nb", 5, "a\n\nb" },
        // T19: line break kinds and trailing break
        { "No utensils\r\nExtra napkins", 20, "No utensils\nExtra napkins" },
        { "a\rb", 5, "a\nb" },
        { "a\r\n\r\nb", 5, "a\n\nb" },
        { "No utensils\n", 20, "No utensils\n" },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void WordWrap_WithText_ReturnsExpectedWrapping(string text, int limit, string expected) =>
        WordWrapper.WordWrap(text, limit).Should().Be(expected);

    [Theory]
    [MemberData(nameof(Cases))]
    public void WordWrap_WithAnyCase_KeepsEveryLineWithinLimitAndTrimmed(string text, int limit, string expected)
    {
        _ = expected;

        var result = WordWrapper.WordWrap(text, limit);

        result.Should().NotContain("\r");
        foreach (var line in result.Split('\n'))
        {
            line.Length.Should().BeLessThanOrEqualTo(limit);
            line.Should().NotStartWith(" ").And.NotStartWith("\t").And.NotEndWith(" ").And.NotEndWith("\t");
        }
    }

    [Fact]
    public void WordWrap_WithNullText_Throws()
    {
        // null! is deliberate: the test passes null to prove the guard.
        var act = () => WordWrapper.WordWrap(null!, 10);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void WordWrap_WithLimitBelowOne_Throws(int limit)
    {
        var act = () => WordWrapper.WordWrap("text", limit);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
