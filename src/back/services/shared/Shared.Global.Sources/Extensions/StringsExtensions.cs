using System.Globalization;

namespace Shared.Global.Sources.Extensions;

public static class StringsExtensions
{
    public static string NormalizeToFTS(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.ToLowerInvariant();

        Span<char> buffer = stackalloc char[value.Length];
        RemoveDiacritics(value.AsSpan(), buffer, out int length);
        var normalizedSpan = buffer.Slice(0, length);

        Span<char> cleanedBuffer = stackalloc char[length];
        CleanAndTrim(normalizedSpan, cleanedBuffer, out int cleanedLength);

        return new string(cleanedBuffer.Slice(0, cleanedLength));
    }

    private static void RemoveDiacritics(ReadOnlySpan<char> input, Span<char> output, out int length)
    {
        length = 0;

        foreach (var c in input)
            if (!CharUnicodeInfo.GetUnicodeCategory(c).Equals(UnicodeCategory.NonSpacingMark))
                output[length++] = c;
    }

    private static void CleanAndTrim(ReadOnlySpan<char> input, Span<char> output, out int length)
    {
        length = 0;
        bool isPrevSpace = true;

        foreach (var c in input)
            if (char.IsWhiteSpace(c))
                if (!isPrevSpace)
                {
                    output[length++] = ' ';
                    isPrevSpace = true;
                }
            else if (char.IsLetterOrDigit(c))
            {
                output[length++] = c;
                isPrevSpace = false;
            }

        if (length > 0 && output[length - 1] == ' ')
            length--;
    }
}
