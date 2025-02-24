namespace PreciseCalc.Tests;

public class ConstructiveRealExtraTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(100)]
    [InlineData(-100)]
    public void TestAssumeInt_Integer(int input)
    {
        var constructiveReal = ConstructiveReal.FromInt(input);
        var result = constructiveReal.AssumeInt();
        Assert.Equal(0, result.CompareTo(constructiveReal, -50));
    }

    [Theory]
    [InlineData(0.5, 1.0)]
    [InlineData(-0.5, -1.0)]
    [InlineData(1.1, 1.0)]
    [InlineData(-2.9, -3.0)]
    public void AssumeInt_Double(double input, double expected)
    {
        var constructiveReal = ConstructiveReal.FromDouble(input);
        var result = constructiveReal.AssumeInt();
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(expected), -50));
    }

    [Fact]
    public void TestAtanPI()
    {
        var result = ConstructiveReal.AtanPI;
        var expected = ConstructiveReal.FromDouble(Math.PI);
        Assert.Equal(0, result.CompareTo(expected, -50));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(123456789)]
    [InlineData(-987654321)]
    public void TestFromLong(long input)
    {
        var result = ConstructiveReal.FromLong(input);
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromInt((int)input), -50));
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(1.0f)]
    [InlineData(-1.5f)]
    [InlineData(3.1415927f)] // π に近い値
    [InlineData(-2.7182818f)] // e に近い値
    public void TestFromFloat(float input)
    {
        var result = ConstructiveReal.FromFloat(input);
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(input), -50));
    }

    [Theory]
    [InlineData("3.1415926535", 10, 3.1415926535)]
    [InlineData("-2.71828", 10, -2.71828)]
    [InlineData("0.5", 10, 0.5)]
    [InlineData("123456789", 10, 123456789)]
    [InlineData("-987654321", 10, -987654321)]
    [InlineData("", 10, 0)]
    public void TestFromString(string input, int radix, double expected)
    {
        var result = ConstructiveReal.FromString(input, radix);
        Assert.Equal(0, result.CompareTo(ConstructiveReal.FromDouble(expected), -50));
    }

    [Theory]
    [InlineData("abc", 10, typeof(FormatException))]
    [InlineData("3.14", 1, typeof(ArgumentException))]
    public void TestFromStringThrowsException(string input, int radix, Type exceptionType)
    {
        Assert.Throws(exceptionType, () => ConstructiveReal.FromString(input, radix));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(127, 127)]
    [InlineData(255, 255)]
    public void TestByteValue(int input, byte expected)
    {
        var constructiveReal = ConstructiveReal.FromInt(input);
        var result = constructiveReal.ByteValue();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(256)]
    [InlineData(1000)]
    [InlineData(-100)]
    public void TestByteValueThrows(int input)
    {
        var constructiveReal = ConstructiveReal.FromInt(input);
        Assert.Throws<OverflowException>(() => constructiveReal.ByteValue());
    }

    [Theory]
    [InlineData(1.0, 1.0, -50, -50, 0)]
    [InlineData(1.0, 1.0001, -10, -10, 0)]
    [InlineData(1.0, 1.1, -50, -50, -1)]
    [InlineData(-1.1, -1.0, -50, -50, -1)]
    public void TestCompareTo(double a, double b, int relPrecision, int absPrecision, int expected)
    {
        var valA = ConstructiveReal.FromDouble(a);
        var valB = ConstructiveReal.FromDouble(b);
        var result = valA.CompareTo(valB, relPrecision, absPrecision);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(3.1415926535, 5, 10, -1, "31416E1")]
    [InlineData(-0.0000001, 6, 10, -7, "-100000E-6")]
    [InlineData(5.625, 4, 2, -10, "1011E3(radix 2)")]
    [InlineData(123.456, 1, 10, -10, "1E3")]
    public void TestToStringFloatRep(double value, int precision, int radix, int minPrecision, string expected)
    {
        var constructiveReal = ConstructiveReal.FromDouble(value);
        var result = constructiveReal.ToStringFloatRep(precision, radix, minPrecision);
        Assert.Equal(expected, result.ToString());
    }
}