using CopilotDemo;

namespace CopilotDemo.Tests;

public class StringUtilitiesTests
{
    [Fact]
    public void Reverse_SimpleString_ReturnsReversed()
    {
        var result = StringUtilities.Reverse("Hello");
        Assert.Equal("olleH", result);
    }

    [Fact]
    public void Reverse_EmptyString_ReturnsEmpty()
    {
        var result = StringUtilities.Reverse("");
        Assert.Equal("", result);
    }

    [Fact]
    public void IsPalindrome_ValidPalindrome_ReturnsTrue()
    {
        Assert.True(StringUtilities.IsPalindrome("racecar"));
    }

    [Fact]
    public void IsPalindrome_CaseInsensitive_ReturnsTrue()
    {
        Assert.True(StringUtilities.IsPalindrome("Race Car"));
    }

    [Fact]
    public void IsPalindrome_NotPalindrome_ReturnsFalse()
    {
        Assert.False(StringUtilities.IsPalindrome("hello"));
    }

    [Theory]
    [InlineData("The quick brown fox", 4)]
    [InlineData("Hello", 1)]
    [InlineData("", 0)]
    [InlineData(null, 0)]
    public void WordCount_VariousInputs_ReturnsExpectedCount(string? input, int expected)
    {
        var result = StringUtilities.WordCount(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Truncate_LongString_TruncatesWithEllipsis()
    {
        var result = StringUtilities.Truncate("Hello World", 7);
        Assert.Equal("Hello W...", result);
    }

    [Fact]
    public void Truncate_ShortString_ReturnsUnchanged()
    {
        var result = StringUtilities.Truncate("Hi", 10);
        Assert.Equal("Hi", result);
    }

    [Fact]
    public void ToTitleCase_LowerCaseWords_CapitalizesEachWord()
    {
        var result = StringUtilities.ToTitleCase("hello world");
        Assert.Equal("Hello World", result);
    }
}
