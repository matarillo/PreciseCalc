namespace PreciseCalc.Tests;

public class ConstructiveRealTest
{
    private readonly ConstructiveReal _zero = ConstructiveReal.FromInt(0);
    private readonly ConstructiveReal _one = ConstructiveReal.FromInt(1);
    private readonly ConstructiveReal _two = ConstructiveReal.FromInt(2);
    private readonly ConstructiveReal _three;
    private readonly ConstructiveReal _four;
    private readonly ConstructiveReal _thirteen = ConstructiveReal.FromInt(13);
    private readonly ConstructiveReal _halfPi;

    public ConstructiveRealTest()
    {
        _three = _two + (_one);
        _four = _two + (_two);
        _halfPi = ConstructiveReal.PI / (_two);
    }

    [Fact]
    public void TestSignum()
    {
        Assert.Equal(1, _one.Sign());
        Assert.Equal(-1, (-_one).Sign());
        Assert.Equal(0, _zero.Sign(-100));
    }

    [Fact]
    public void TestComparison()
    {
        Assert.Equal(-1, _one.CompareTo(_two, -10));
    }

    [Fact]
    public void TestToString()
    {
        Assert.Equal("2.0000", _two.ToString(4));
    }

    [Fact]
    public void TestShiftOperations()
    {
        Assert.Equal(0, (_one << (1)).CompareTo(_two, -50));
        Assert.Equal(0, (_two >> (1)).CompareTo(_one, -50));
    }

    [Fact]
    public void TestAddition()
    {
        Assert.Equal(0, (_one + _one).CompareTo(_two, -50));
        Assert.Equal(0, ConstructiveReal.FromInt(4).CompareTo(_four, -50));
        Assert.Equal(0, ConstructiveReal.FromInt(3).CompareTo(_three, -50));
        Assert.Equal(0, ((-_one) + (_two)).CompareTo(_one, -50));
    }

    [Fact]
    public void TestMinMax()
    {
        Assert.Equal(0, _one.Max(_two).CompareTo(_two, -50));
        Assert.Equal(0, _one.Min(_two).CompareTo(_one, -50));
    }

    [Fact]
    public void TestAbs()
    {
        Assert.Equal(0, (-_one).Abs().CompareTo(_one, -50));
    }

    [Fact]
    public void TestNegate()
    {
        Assert.Equal(-1, (-_one).Sign());
    }

    [Fact]
    public void TestMultiplication()
    {
        Assert.Equal(0, (_two * _two).CompareTo(_four, -50));
    }

    [Fact]
    public void TestDivision()
    {
        Assert.Equal(0, ((_one / (_four)) << (4)).CompareTo(_four, -50));
        Assert.Equal(0, (_two / (-_one)).CompareTo(-_two, -50));
        Assert.Equal(0, ((_one / (_thirteen)) * (_thirteen)).CompareTo(_one, -50));
    }

    [Fact]
    public void TestConversions()
    {
        Assert.Equal(13.0f, _thirteen.FloatValue(), 0.0);
        Assert.Equal(13, _thirteen.IntValue());
        Assert.Equal(13L, _thirteen.LongValue());
        Assert.Equal(13.0, _thirteen.DoubleValue(), 0.0);
    }

    [Fact]
    public void TestExp()
    {
        Assert.Equal(0, _zero.Exp().CompareTo(_one, -50));
        string eString = _one.Exp().ToString(20);
        Assert.StartsWith("2.718281828459045", eString);
    }

    [Fact]
    public void TestLn()
    {
        Assert.Equal(0, _one.Ln().CompareTo(_zero, -50));
    }

    [Fact]
    public void TestTrigFunctions()
    {
        Assert.Equal(0, _halfPi.Sin().CompareTo(_one, -50));
        Assert.Equal(0, ConstructiveReal.PI.Cos().CompareTo(-_one, -50));
    }

    [Fact]
    public void TestIntermediateConversions()
    {
        ConstructiveReal tmp = ConstructiveReal.PI + (ConstructiveReal.FromInt(-123).Exp());
        ConstructiveReal tmp2 = tmp - (ConstructiveReal.PI);
        Assert.Equal(-123, tmp2.Ln().IntValue());
        Assert.Equal(-123L, tmp2.Ln().LongValue());
        Assert.Equal(-123.0f, tmp2.Ln().FloatValue(), 0.0);
        Assert.Equal(-123.0, tmp2.Ln().DoubleValue(), 0.0);
    }

    [Fact]
    public void TestDoubleRangeForSin()
    {
        for (double n = -10.0; n < 10.0; n += 2.0)
        {
            Assert.Equal(Math.Sin(n), ConstructiveReal.FromDouble(n).Sin().DoubleValue(), 0.000001);
        }
    }

    [Fact]
    public void TestDoubleRangeForCos()
    {
        for (double n = -10.0; n < 10.0; n += 2.0)
        {
            Assert.Equal(Math.Cos(n), ConstructiveReal.FromDouble(n).Cos().DoubleValue(), 0.000001);
        }
    }

    [Fact]
    public void TestDoubleRangeForExp()
    {
        for (double n = -10.0; n < 10.0; n += 2.0)
        {
            Assert.Equal(Math.Exp(n), ConstructiveReal.FromDouble(n).Exp().DoubleValue(), 0.000001);
        }
    }

    [Fact]
    public void TestPositiveDoubleRangeForExp()
    {
        for (double n = 2.0; n < 10.0; n += 2.0)
        {
            Assert.Equal(Math.Exp(n), ConstructiveReal.FromDouble(n).Exp().DoubleValue(), 0.000001);
        }
    }

    [Fact]
    public void TestLargeNumberCos()
    {
        Assert.Equal(Math.Cos(12345678.0), ConstructiveReal.FromInt(12345678).Cos().DoubleValue(), 0.000001);
    }
}