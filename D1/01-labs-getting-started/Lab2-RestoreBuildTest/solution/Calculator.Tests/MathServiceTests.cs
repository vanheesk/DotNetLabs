using Calculator;

namespace Calculator.Tests;

public class MathServiceTests
{
    [Fact]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        var result = MathService.Add(3, 5);
        Assert.Equal(8, result);
    }

    [Fact]
    public void Add_NegativeAndPositive_ReturnsCorrectResult()
    {
        var result = MathService.Add(-3, 5);
        Assert.Equal(2, result);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 2)]
    [InlineData(-5, 5, 0)]
    [InlineData(100, 200, 300)]
    public void Add_VariousInputs_ReturnsExpectedSum(int a, int b, int expected)
    {
        var result = MathService.Add(a, b);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Subtract_ReturnsCorrectDifference()
    {
        var result = MathService.Subtract(10, 3);
        Assert.Equal(7, result);
    }

    [Fact]
    public void Multiply_ReturnsCorrectProduct()
    {
        var result = MathService.Multiply(4, 5);
        Assert.Equal(20, result);
    }

    [Fact]
    public void Divide_ByNonZero_ReturnsCorrectQuotient()
    {
        var result = MathService.Divide(10, 2);
        Assert.Equal(5, result);
    }

    [Fact]
    public void Divide_ByZero_ThrowsException()
    {
        Assert.Throws<DivideByZeroException>(() => MathService.Divide(10, 0));
    }
}
