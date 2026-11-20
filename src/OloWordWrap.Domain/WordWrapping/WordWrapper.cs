using System.Text;

namespace OloWordWrap.Domain.WordWrapping;

public static class WordWrapper
{
    private static readonly char[] _wordSeparators = [' ', '\t'];

    /// <summary>
    /// Wraps text to lines of at most <paramref name="limit"/> characters: a word that exactly fits stays on the line,
    /// typed line breaks (<c>\n</c>, <c>\r\n</c>, <c>\r</c>) are kept, and lines are joined with <c>\n</c>.
    /// A word longer than the limit is split over lines: each broken line is <paramref name="limit"/> characters
    /// including a trailing <c>-</c> (no second hyphen if the piece already ends in one; a <paramref name="limit"/>
    /// of 1 has no room for one, so the word is split without hyphens).
    /// </summary>
    public static string WordWrap(string text, int limit)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);

        if (string.IsNullOrWhiteSpace(text))
        {
            return "";
        }

        var paragraphs = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var lines = new List<string>();
        foreach (var paragraph in paragraphs)
        {
            WrapParagraph(paragraph, limit, lines);
        }

        return string.Join('\n', lines);
    }

    private static void WrapParagraph(string paragraph, int limit, List<string> lines)
    {
        var words = paragraph.Split(_wordSeparators, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
        {
            lines.Add("");
            return;
        }

        var current = new StringBuilder();
        foreach (var word in words)
        {
            if (current.Length > 0 && current.Length + 1 + word.Length <= limit)
            {
                current.Append(' ').Append(word);
                continue;
            }

            FlushLine(current, lines);
            if (word.Length <= limit)
            {
                current.Append(word);
            }
            else
            {
                AddLongWordChunks(word, limit, lines, current);
            }
        }

        FlushLine(current, lines);
    }

    // Emits full-size chunks; the last one stays open so following words can join it.
    // Each chunk is one character shorter than the limit to leave room for a "-" (unless limit is 1).
    private static void AddLongWordChunks(string word, int limit, List<string> lines, StringBuilder current)
    {
        var addHyphen = limit > 1;
        var chunkSize = addHyphen ? limit - 1 : limit;
        var start = 0;
        while (word.Length - start > limit)
        {
            // A hyphen already at the limit is the break: keep it instead of adding one before it.
            var size = addHyphen && word[start + limit - 1] == '-' ? limit : chunkSize;
            var chunk = word.Substring(start, size);
            lines.Add(addHyphen && !chunk.EndsWith('-') ? chunk + "-" : chunk);
            start += size;
        }

        current.Append(word, start, word.Length - start);
    }

    private static void FlushLine(StringBuilder current, List<string> lines)
    {
        if (current.Length == 0)
        {
            return;
        }

        lines.Add(current.ToString());
        current.Clear();
    }
}
