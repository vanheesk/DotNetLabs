namespace Calculator;

public static class MathService
{
    public static int Add(int a, int b) => a + b;

    public static int Subtract(int a, int b) => a - b;

    public static int Multiply(int a, int b) => a * b;

    public static int Divide(int a, int b) =>
        b == 0 ? throw new DivideByZeroException("Cannot divide by zero.") : a / b;
}
