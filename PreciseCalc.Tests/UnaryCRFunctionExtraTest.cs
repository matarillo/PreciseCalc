namespace PreciseCalc.Tests;

public class UnaryCRFunctionExtraTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, -5)]
    [InlineData(-3, 3)]
    [InlineData(1000000, -1000000)]
    public void TestNegate(int input, int expected)
    {
        var result = UnaryCRFunction.NegateFunction.Execute(ConstructiveReal.FromInt(input));
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromInt(expected), -50));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(1000000)]
    [InlineData(-1000000)]
    public void TestIdentity(int input)
    {
        var result = UnaryCRFunction.IdentityFunction.Execute(ConstructiveReal.FromInt(input));
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromInt(input), -50));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(-1, -1)]
    [InlineData(2, 0.5)]
    [InlineData(0.5, 2)]
    [InlineData(1000000, 0.000001)]
    public void TestInverse(double input, double expected)
    {
        var result = UnaryCRFunction.InverseFunction.Execute(ConstructiveReal.FromDouble(input));
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(expected), -50));
    }

    [Fact]
    public void TestInverseZero()
    {
        var zero = ConstructiveReal.Zero;
        var inf = UnaryCRFunction.InverseFunction.Execute(zero);
        Assert.Throws<PrecisionOverflowException>(() => inf.DoubleValue());
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(-5, 5)]
    [InlineData(1000000, 1000000)]
    [InlineData(-1000000, 1000000)]
    [InlineData(0.001, 0.001)]
    [InlineData(-0.001, 0.001)]
    public void TestAbs(double input, double expected)
    {
        var result = UnaryCRFunction.AbsFunction.Execute(ConstructiveReal.FromDouble(input));
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(expected), -50));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2.718281828459045)]
    [InlineData(-1, 0.36787944117144233)]
    [InlineData(2, 7.38905609893065)]
    [InlineData(-2, 0.1353352832366127)]
    public void TestExp(double input, double expected)
    {
        var result = UnaryCRFunction.ExpFunction.Execute(ConstructiveReal.FromDouble(input));
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(expected), -50));
    }

    [Fact]
    public void TestExpLargeNumber()
    {
        // e^10 does not throw
        var normalValue = ConstructiveReal.FromDouble(10);
        var result1 = UnaryCRFunction.ExpFunction.Execute(normalValue);
        Assert.Equal("22026.46579480671651695790", result1.ToString(20));

        // e^100 does not throw
        var largeValue = ConstructiveReal.FromDouble(100);
        var result2 = UnaryCRFunction.ExpFunction.Execute(largeValue);
        Assert.Equal("26881171418161354484126255515800135873611118.77374192241519160862", result2.ToString(20));
    }

    [Theory]
    [InlineData(0, 1)] // cos(0) = 1
    [InlineData(1.57079632679, 0)] // cos(π/2) ≈ 0
    [InlineData(3.14159265359, -1)] // cos(π) = -1
    [InlineData(6.28318530718, 1)] // cos(2π) = 1
    [InlineData(-1.57079632679, 0)] // cos(-π/2) ≈ 0
    [InlineData(-3.14159265359, -1)] // cos(-π) = -1
    public void TestCos(double input, double expected)
    {
        var result = UnaryCRFunction.CosFunction.Execute(ConstructiveReal.FromDouble(input));
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(expected), -30));
    }

    [Fact]
    public void TestCosExpLargeNumber()
    {
        // cos(1000000) does not throw
        var largeValue = ConstructiveReal.FromDouble(1000000);
        var result = UnaryCRFunction.CosFunction.Execute(largeValue);
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(1, 0)] // ln(1) = 0
    [InlineData(2.718281828459045, 1)] // ln(e) ≈ 1
    [InlineData(7.38905609893065, 2)] // ln(e^2) ≈ 2
    [InlineData(0.36787944117144233, -1)] // ln(1/e) ≈ -1
    [InlineData(0.1353352832366127, -2)] // ln(1/e^2) ≈ -2
    [InlineData(1000000, 13.815510557964274)] // ln(1000000) ≈ 13.81
    public void TestLn(double input, double expected)
    {
        var result = UnaryCRFunction.LnFunction.Execute(ConstructiveReal.FromDouble(input));
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(expected), -30));
    }

    [Fact]
    public void TestLnZero()
    {
        var zero = ConstructiveReal.Zero;
        Assert.Throws<PrecisionOverflowException>(() => UnaryCRFunction.LnFunction.Execute(zero));
    }

    [Fact]
    public void TestLnNegativeNumber()
    {
        var negative = ConstructiveReal.FromDouble(-1);
        Assert.Throws<ArithmeticException>(() => UnaryCRFunction.LnFunction.Execute(negative));
    }

    [Theory]
    [InlineData(1, 0)] // ln(e^0) = 0
    [InlineData(2.718281828459045, 1)] // ln(e^1) = 1
    [InlineData(7.38905609893065, 2)] // ln(e^2) = 2
    [InlineData(0.36787944117144233, -1)] // ln(e^-1) = -1
    public void TestInverseMonotone_Exp(double input, double expected)
    {
        var logFunction =
            UnaryCRFunction.ExpFunction.InverseMonotone(ConstructiveReal.FromDouble(-3),
                ConstructiveReal.FromDouble(3));
        var result = logFunction.Execute(ConstructiveReal.FromDouble(input));
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(expected), -50));
    }

    [Theory]
    [InlineData(1, -2.718281828459045)] // Negate(Exp(1)) = -e^1 ≈ -2.718
    [InlineData(2, -7.38905609893065)] // Negate(Exp(2)) = -e^2 ≈ -7.389
    [InlineData(-1, -0.36787944117144233)] // Negate(Exp(-1)) = -e^(-1) ≈ -0.367
    [InlineData(0, -1)] // Negate(Exp(0)) = -1
    public void TestComposeExpThenNegate(double input, double expected)
    {
        var expThenNegate = UnaryCRFunction.NegateFunction.Compose(UnaryCRFunction.ExpFunction);
        var result = expThenNegate.Execute(ConstructiveReal.FromDouble(input));
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(expected), -50));
    }
}