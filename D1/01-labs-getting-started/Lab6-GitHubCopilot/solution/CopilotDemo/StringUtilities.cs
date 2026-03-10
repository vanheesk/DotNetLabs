namespace CopilotDemo;

/// <summary>
/// A collection of string utility methods (SOLUTION).
/// </summary>
public static class StringUtilities
{
    public static string Reverse(string input)
    {
        var chars = input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public static bool IsPalindrome(string input)
    {
        var cleaned = new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();
        var reversed = Reverse(cleaned);
        return cleaned == reversed;
    }

    public static int WordCount(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return 0;
        }

        return input.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    public static string Truncate(string input, int maxLength)
    {
        if (input.Length <= maxLength)
        {
            return input;
        }

        return input[..maxLength] + "...";
    }

    public static string ToTitleCase(string input)
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}
