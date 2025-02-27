using System.Numerics;

namespace PreciseCalc.Tests;

public class UnaryCRFunctionTest
{
    private readonly ConstructiveReal _zero = ConstructiveReal.Zero;
    private readonly ConstructiveReal _one = ConstructiveReal.One;
    private readonly ConstructiveReal _two = ConstructiveReal.FromInt(2);
    private readonly ConstructiveReal _half;
    private readonly ConstructiveReal _halfPi;
    private readonly ConstructiveReal _three = ConstructiveReal.FromInt(3);
    private readonly ConstructiveReal _thirteen = ConstructiveReal.FromInt(13);
    private readonly ConstructiveReal _huge;

    private readonly UnaryCRFunction _asin = UnaryCRFunction.AsinFunction;
    private readonly UnaryCRFunction _acos = UnaryCRFunction.AcosFunction;
    private readonly UnaryCRFunction _atan = UnaryCRFunction.AtanFunction;
    private readonly UnaryCRFunction _tan = UnaryCRFunction.TanFunction;
    private readonly UnaryCRFunction _sqrt = UnaryCRFunction.SqrtFunction;

    public UnaryCRFunctionTest()
    {
        _half = _one.Divide(_two);
        _halfPi = ConstructiveReal.PI.Divide(_two);
        BigInteger million = 1000 * 1000;
        BigInteger thousand = 1000;
        _huge = ConstructiveReal.FromBigInteger(million * million * thousand);
    }

    private const int Precision = -50;

    [Fact]
    public void TestAsin()
    {
        Assert.Equal(0, _asin.Execute(_one).CompareTo(_halfPi, Precision));
        Assert.Equal(0, _asin.Execute(_one.Negate()).CompareTo(_halfPi.Negate(), Precision));
        Assert.Equal(0, _asin.Execute(_zero).CompareTo(_zero, Precision));
        Assert.Equal(0, _asin.Execute(_half.Sin()).CompareTo(_half, Precision));
        Assert.Equal(0, _asin.Execute(_one.Sin()).CompareTo(_one, Precision));
    }

    [Fact]
    public void TestMonotoneDerivative()
    {
        UnaryCRFunction cosine = UnaryCRFunction.SinFunction.MonotoneDerivative(_zero, ConstructiveReal.PI);
        Assert.Equal(0, cosine.Execute(_one).CompareTo(_one.Cos(), Precision));
        Assert.Equal(0, cosine.Execute(_three).CompareTo(_three.Cos(), Precision));
    }

    [Fact]
    public void TestAcos()
    {
        Assert.Equal(0, _acos.Execute(_one.Cos()).CompareTo(_one, Precision));
    }

    [Fact]
    public void TestAtanTan()
    {
        Assert.Equal(0, _atan.Execute(_tan.Execute(_one)).CompareTo(_one, Precision));
        Assert.Equal(0, _atan.Execute(_tan.Execute(_one.Negate())).CompareTo(_one.Negate(), Precision));
        Assert.Equal(0, _tan.Execute(_atan.Execute(_huge)).CompareTo(_huge, Precision));
    }

    [Fact]
    public void TestSqrt()
    {
        ConstructiveReal sqrt13 = _sqrt.Execute(_thirteen);
        Assert.Equal(0, sqrt13.Multiply(sqrt13).CompareTo(_thirteen, Precision));
    }
}