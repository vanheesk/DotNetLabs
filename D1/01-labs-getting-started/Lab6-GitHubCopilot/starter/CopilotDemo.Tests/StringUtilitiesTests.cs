using CopilotDemo;

namespace CopilotDemo.Tests;

/// <summary>
/// Unit tests for StringUtilities.
/// Use GitHub Copilot to implement the test methods based on the TODO comments.
/// </summary>
public class StringUtilitiesTests
{
    [Fact]
    public void Reverse_SimpleString_ReturnsReversed()
    {
        // TODO: Use Copilot to implement this test
        // Test that "Hello" reversed is "olleH"
        throw new NotImplementedException();
    }

    [Fact]
    public void Reverse_EmptyString_ReturnsEmpty()
    {
        // TODO: Use Copilot to implement this test
        throw new NotImplementedException();
    }

    [Fact]
    public void IsPalindrome_ValidPalindrome_ReturnsTrue()
    {
        // TODO: Use Copilot to implement this test
        // Test with "racecar"
        throw new NotImplementedException();
    }

    [Fact]
    public void IsPalindrome_CaseInsensitive_ReturnsTrue()
    {
        // TODO: Use Copilot to implement this test
        // Test with "Race Car"
        throw new NotImplementedException();
    }

    [Fact]
    public void IsPalindrome_NotPalindrome_ReturnsFalse()
    {
        // TODO: Use Copilot to implement this test
        throw new NotImplementedException();
    }

    [Theory]
    [InlineData("The quick brown fox", 4)]
    [InlineData("Hello", 1)]
    [InlineData("", 0)]
    [InlineData(null, 0)]
    public void WordCount_VariousInputs_ReturnsExpectedCount(string? input, int expected)
    {
        // TODO: Use Copilot to implement this test
        throw new NotImplementedException();
    }

    [Fact]
    public void Truncate_LongString_TruncatesWithEllipsis()
    {
        // TODO: Use Copilot to implement this test
        throw new NotImplementedException();
    }

    [Fact]
    public void Truncate_ShortString_ReturnsUnchanged()
    {
        // TODO: Use Copilot to implement this test
        throw new NotImplementedException();
    }

    [Fact]
    public void ToTitleCase_LowerCaseWords_CapitalizesEachWord()
    {
        // TODO: Use Copilot to implement this test
        throw new NotImplementedException();
    }
}
