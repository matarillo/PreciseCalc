using System.Diagnostics;
using System.Numerics;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PreciseCalc.Tests;

public class UnifiedRealVsFpTimer(ITestOutputHelper testOutputHelper)
{
    private const int Wrong = 3;
    private const int ComparePrecision = -2000;
    private static readonly UnifiedReal Three = UnifiedReal.FromLong(3);
    private static readonly UnifiedReal MinusSeven = UnifiedReal.FromLong(-17);
    private static readonly Random Rand = new();
    private int _ops;

    private Stopwatch _stopwatch = new();

    private static void CheckComparable(UnifiedReal x, UnifiedReal y)
    {
        if (!x.IsComparable(y))
            throw new XunitException($"{x.ToDisplayString()} not comparable to {y.ToDisplayString()}");
    }

    /// <summary>
    ///     Return the difference between fpVal and urVal in ulps. 0 ==> correctly rounded. fpVal is a/the
    ///     closest representable value fp value. 1 ==> within 1 ulp. fpVal is either the next higher or
    ///     next lower fp value. If the exact answer is representable, then fpVal is exactly urVal, and
    ///     hence we would have returned 0, not 1. 2 ==> within 2 ulps. fpVal is one removed from the next
    ///     higher or lower fpVal. WRONG ==> More than 2 ulps error. We optimistically assume that either
    ///     urVal is known to be rational, or urVal is irrational, and thus all of our comparisons will
    ///     converge. In a few cases below, we explicitly avoid empirically observed divergence resulting
    ///     from violation of this assumption.
    /// </summary>
    private static int UlpError(double fpVal, UnifiedReal urVal)
    {
        var fpAsUr = UnifiedReal.FromDouble(fpVal);
        CheckComparable(fpAsUr, urVal);
        var errorSign = fpAsUr.CompareTo(urVal);

        if (errorSign == 0) return 0; // Exactly equal.

        if (errorSign < 0) return UlpError(-fpVal, -urVal);

        // errorSign > 0
        var prevFp = Math.BitDecrement(fpVal);
        if (double.IsNegativeInfinity(prevFp))
            // Most negative representable value was returned. True result is smaller.
            // That seems to qualify as "correctly rounded".
            return 0;

        var prev = UnifiedReal.FromDouble(prevFp);
        CheckComparable(prev, urVal);

        if (prev.CompareTo(urVal) >= 0)
        {
            // prev is a better approximation.
            var prevprevFp = Math.BitDecrement(prevFp);
            if (double.IsNegativeInfinity(prevprevFp)) return 2; // Dubious, but seems to qualify.

            var prevprev = UnifiedReal.FromDouble(prevprevFp);
            CheckComparable(prevprev, urVal);

            if (prevprev.CompareTo(urVal) >= 0)
                // urVal <= prevprev < prev < fpVal. fpVal is neither one of the
                // bracketing values, nor one next to it.
                return Wrong;

            return 2;
        }

        var prevDiff = urVal - prev;
        var fpValDiff = fpAsUr - urVal;
        CheckComparable(fpValDiff, prevDiff);

        if (fpValDiff.CompareTo(prevDiff) <= 0) return 0;

        return 1;
    }

    /// <summary>
    ///     Return the difference between fpVal and urVal in ulps. Behaves like ulpError(),
    ///     but accommodates situations in which urVal is not known comparable with rationals.
    ///     In that case the answer could conceivably be wrong, though we evaluate to sufficiently
    ///     high precision to make that unlikely.
    /// </summary>
    private static int ApproxUlpError(double fpVal, UnifiedReal urVal)
    {
        var fpAsUr = UnifiedReal.FromDouble(fpVal);
        var errorSign = fpAsUr.CompareTo(urVal, ComparePrecision);

        if (errorSign == 0) return 0; // Exactly equal.

        if (errorSign < 0) return ApproxUlpError(-fpVal, -urVal);

        // errorSign > 0
        var prevFp = Math.BitDecrement(fpVal);
        if (double.IsNegativeInfinity(prevFp))
            // Most negative representable value was returned. True result is smaller.
            // That seems to qualify as "correctly rounded".
            return 0;

        var prev = UnifiedReal.FromDouble(prevFp);

        if (prev.CompareTo(urVal, ComparePrecision) >= 0)
        {
            // prev is a better approximation.
            var prevprevFp = Math.BitDecrement(prevFp);
            if (double.IsNegativeInfinity(prevprevFp)) return 2; // Dubious, but seems to qualify.

            var prevprev = UnifiedReal.FromDouble(prevprevFp);

            if (prevprev.CompareTo(urVal, ComparePrecision) >= 0)
                // urVal <= prevprev < prev < fpVal. fpVal is neither one of the
                // bracketing values, nor one next to it.
                return Wrong;

            return 2;
        }

        var prevDiff = urVal - prev;
        var fpValDiff = fpAsUr - urVal;

        if (fpValDiff.CompareTo(prevDiff, ComparePrecision) <= 0) return 0;

        return 1;
    }

    private static UnifiedReal Hypot(UnifiedReal x, UnifiedReal y)
    {
        return (x * x + y * y).Sqrt();
    }

    /// <summary>
    ///     Generate a random double such that all bit patterns representing a finite value are equally
    ///     likely. Do not generate a NaN or Infinite result.
    /// </summary>
    private static double GetRandomDouble()
    {
        double result;
        do
        {
            result = BitConverter.Int64BitsToDouble(Rand.NextInt64());
        } while (double.IsNaN(result) || double.IsInfinity(result));

        return result;
    }

    /// <summary>
    ///     Check that basic Math functions obey stated error bounds on argument x. x is assumed to be
    ///     finite. We assume that the UnifiedReal functions produce known rational results when the
    ///     results are rational.
    /// </summary>
    private void CheckDivAt(double x, double other)
    {
        if (!double.IsFinite(x)) return;

        if (x != 0.0)
        {
            var xAsUr = UnifiedReal.FromDouble(x);

            if (!double.IsInfinity(other / x))
            {
                var otherAsUr = UnifiedReal.FromDouble(other);
                _ops++;
                Assert.Equal(0, UlpError(other / x, otherAsUr.Divide(xAsUr)));
            }
        }
    }

    private void CheckExpAt(double x)
    {
        if (!double.IsFinite(x)) return;

        var result = Math.Exp(x);

        if (result != 0 && !double.IsInfinity(result))
        {
            _ops++;
            var xAsUr = UnifiedReal.FromDouble(x);
            Assert.True(UlpError(result, xAsUr.Exp()) <= 1);
        } // Otherwise the UnifiedReal computation may be intractable.
    }

    private void CheckLnAt(double x)
    {
        if (!double.IsFinite(x) || x <= 0.0) return;

        var result = Math.Log(x);
        _ops++;
        var xAsUr = UnifiedReal.FromDouble(x);
        Assert.True(UlpError(result, xAsUr.Ln()) <= 1);
    }

    private void CheckLogAt(double x)
    {
        if (!double.IsFinite(x) || x <= 0.0) return;

        var result = Math.Log10(x);
        _ops++;
        var xAsUr = UnifiedReal.FromDouble(x);
        Assert.True(UlpError(result, xAsUr.Log()) <= 1);
    }

    private void CheckSqrtAt(double x)
    {
        if (!double.IsFinite(x)) return;

        if (x >= 0)
        {
            _ops++;
            var rt = Math.Sqrt(x);
            var xAsUr = UnifiedReal.FromDouble(x);
            var urRt = xAsUr.Sqrt();
            Assert.Equal(0, UlpError(rt, urRt));
        }
    }

    private void CheckSinAt(double x)
    {
        if (!double.IsFinite(x)) return;

        _ops++;
        var xAsUr = UnifiedReal.FromDouble(x);
        Assert.True(UlpError(Math.Sin(x), xAsUr.Sin()) <= 1);
    }

    private void CheckCosAt(double x)
    {
        if (!double.IsFinite(x)) return;

        _ops++;
        var xAsUr = UnifiedReal.FromDouble(x);
        Assert.True(UlpError(Math.Cos(x), xAsUr.Cos()) <= 1);
    }

    private void CheckTanAt(double x)
    {
        if (!double.IsFinite(x)) return;

        _ops++;
        var xAsUr = UnifiedReal.FromDouble(x);
        Assert.True(UlpError(Math.Tan(x), xAsUr.Tan()) <= 1);
    }

    private void CheckAtanAt(double x)
    {
        if (!double.IsFinite(x)) return;

        _ops++;
        var xAsUr = UnifiedReal.FromDouble(x);
        Assert.True(UlpError(Math.Atan(x), xAsUr.Atan()) <= 1);
    }

    private void CheckAsinAt(double x)
    {
        if (!double.IsFinite(x) || Math.Abs(x) > 1) return;

        _ops++;
        var xAsUr = UnifiedReal.FromDouble(x);
        Assert.True(UlpError(Math.Asin(x), xAsUr.Asin()) <= 1);
    }

    private void CheckAcosAt(double x)
    {
        if (!double.IsFinite(x) || Math.Abs(x) > 1) return;

        _ops++;
        var xAsUr = UnifiedReal.FromDouble(x);
        Assert.True(UlpError(Math.Acos(x), xAsUr.Acos()) <= 1);
    }

    private void CheckHypotAt(double x, double other)
    {
        if (!double.IsFinite(x)) return;

        if (double.IsNaN(other) || double.IsInfinity(other)) return;

        _ops++;
        var h = Hypot(x, other); // C# equivalent to Java's Math.hypot
        var xAsUr = UnifiedReal.FromDouble(x);
        var otherAsUr = UnifiedReal.FromDouble(other);
        var hUr = Hypot(xAsUr, otherAsUr);

        if (double.IsInfinity(h))
        {
            var h2 = hUr.ToDouble();
            Assert.True(
                double.IsInfinity(h2)
                || double.IsInfinity(Math.BitIncrement(h2)));
            // TODO: Since h2 is not yet correctly rounded, this could conceivably still fail
            // spuriously. But that's extremely unlikely.
        }
        else
        {
            Assert.True(UlpError(h, hUr) <= 1);
        }
    }

    private static double Hypot(double x, double y)
    {
        return new Complex(x, y).Magnitude;
    }

    private void CheckPowAt(double x, double other)
    {
        if (!double.IsFinite(x) || !double.IsFinite(other)) return;

        if (x >= 0.0 || other == Math.Round(other))
        {
            var p = Math.Pow(x, other);

            if (!double.IsInfinity(p))
            {
                _ops++;
                var xAsUr = UnifiedReal.FromDouble(x);
                var otherAsUr = UnifiedReal.FromDouble(other);
                var urP = xAsUr.Pow(otherAsUr);

                if (urP.CompareTo(UnifiedReal.FromDouble(p), ComparePrecision) != 0)
                    Assert.True(ApproxUlpError(p, urP) <= 1);
            }
        }
    }

    private void InitTiming()
    {
        _ops = 0;
        _stopwatch = Stopwatch.StartNew();
    }

    private void FinishTiming(string label)
    {
        _stopwatch.Stop();
        testOutputHelper.WriteLine(
            $"{label}: {_ops} checks took {_stopwatch.ElapsedMilliseconds} msecs or {1000.0 * _stopwatch.ElapsedMilliseconds / _ops} usecs/check");
    }

    private void ManyRandomDoubleChecks()
    {
        const int nIters = 100; // TODO: 10000;

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckDivAt(GetRandomDouble(), GetRandomDouble());

        FinishTiming("div");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckExpAt(GetRandomDouble());

        FinishTiming("exp");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckLnAt(GetRandomDouble());

        FinishTiming("ln");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckLogAt(GetRandomDouble());

        FinishTiming("log");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckSqrtAt(GetRandomDouble());

        FinishTiming("sqrt");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckSinAt(GetRandomDouble());

        FinishTiming("sin");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckCosAt(GetRandomDouble());

        FinishTiming("cos");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckTanAt(GetRandomDouble());

        FinishTiming("tan");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckAsinAt(GetRandomDouble());

        FinishTiming("asin");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckAcosAt(GetRandomDouble());

        FinishTiming("acos");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckAtanAt(GetRandomDouble());

        FinishTiming("atan");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckHypotAt(GetRandomDouble(), GetRandomDouble());

        FinishTiming("hypot");

        InitTiming();
        for (var i = 0; i < nIters; i++) CheckPowAt(GetRandomDouble(), GetRandomDouble());

        FinishTiming("pow");

        InitTiming();
        for (var i = 0; i < nIters; i++)
            if (GetRandomDouble() == GetRandomDouble())
                Console.Error.WriteLine("jackpot!");

        FinishTiming("empty");
    }

    [Fact]
    public void Test()
    {
        for (var i = 0; i < 6; ++i) ManyRandomDoubleChecks();
    }
}