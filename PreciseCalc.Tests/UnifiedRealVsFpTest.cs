using System.Numerics;
using Xunit.Abstractions;

namespace PreciseCalc.Tests;

public class UnifiedRealVsFpTest(ITestOutputHelper testOutputHelper)
{
    private const int Wrong = 3;

    private const int ComparePrec = -2000;
    private static readonly UnifiedReal Three = UnifiedReal.FromLong(3);
    private static readonly UnifiedReal Minus17 = UnifiedReal.FromLong(-17);
    private readonly Random _rand = new();

    private static double Hypot(double x, double y)
    {
        return new Complex(x, y).Magnitude;
    }

    private void CheckComparable(UnifiedReal x, UnifiedReal y)
    {
        if (!x.IsComparable(y)) throw new Exception(x.ToDisplayString() + " not comparable to " + y.ToDisplayString());
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
    private int UlpError(double fpVal, UnifiedReal urVal)
    {
        Assert.True(urVal.PropertyCorrect(ComparePrec),
            "Property wrong for " + urVal.ToDisplayString()); // Check UnifiedReal for internal consistency.
        var fpAsUr = UnifiedReal.FromDouble(fpVal);
        CheckComparable(fpAsUr, urVal);
        var errorSign = fpAsUr.CompareTo(urVal);
        if (errorSign == 0) return 0; // Exactly equal.

        if (errorSign < 0) return UlpError(-fpVal, -urVal);

        // errorSign > 0
        var prevFp = Math.BitDecrement(fpVal);
        if (double.IsInfinity(prevFp))
            // Most negative representable value was returned. True result is smaller.
            // That seems to qualify as "correctly rounded".
            return 0;

        var prev = UnifiedReal.FromDouble(prevFp);
        CheckComparable(prev, urVal);
        if (prev.CompareTo(urVal) >= 0)
        {
            // prev is a better approximation.
            var prevprevFp = Math.BitDecrement(prevFp);
            if (double.IsInfinity(prevprevFp)) return 2; // Dubious, but seems to qualify.

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
    private int ApproxUlpError(double fpVal, UnifiedReal urVal)
    {
        Assert.True(urVal.PropertyCorrect(-1000),
            "Property wrong for " + urVal.ToDisplayString()); // Check UnifiedReal for internal consistency.
        var fpAsUr = UnifiedReal.FromDouble(fpVal);
        var errorSign = fpAsUr.CompareTo(urVal, ComparePrec);
        if (errorSign == 0) return 0; // Exactly equal.

        if (errorSign < 0) return ApproxUlpError(-fpVal, -urVal);

        // errorSign > 0
        var prevFp = Math.BitDecrement(fpVal);
        if (double.IsInfinity(prevFp))
            // Most negative representable value was returned. True result is smaller.
            // That seems to qualify as "correctly rounded".
            return 0;

        var prev = UnifiedReal.FromDouble(prevFp);
        if (prev.CompareTo(urVal, ComparePrec) >= 0)
        {
            // prev is a better approximation.
            var prevprevFp = Math.BitDecrement(prevFp);
            if (double.IsInfinity(prevprevFp)) return 2; // Dubious, but seems to qualify.

            var prevprev = UnifiedReal.FromDouble(prevprevFp);
            if (prevprev.CompareTo(urVal, ComparePrec) >= 0)
                // urVal <= prevprev < prev < fpVal. fpVal is neither one of the
                // bracketing values, nor one next to it.
                return Wrong;

            return 2;
        }

        var prevDiff = urVal - prev;
        var fpValDiff = fpAsUr - urVal;
        if (fpValDiff.CompareTo(prevDiff, ComparePrec) <= 0) return 0;

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
    private double GetRandomDouble()
    {
        double result;
        do
        {
            var buffer = new byte[8];
            _rand.NextBytes(buffer);
            result = BitConverter.ToDouble(buffer, 0);
        } while (double.IsNaN(result) || double.IsInfinity(result));

        return result;
    }

    /// <summary>
    ///     Check that basic Math functions obey stated error bounds on argument x. x is assumed to be
    ///     finite. We assume that the UnifiedReal functions produce known rational results when the
    ///     results are rational.
    /// </summary>
    private void CheckFunctionsAt(double x, double other)
    {
        if (double.IsNaN(x) || double.IsInfinity(x)) return;

        var xAsUr = UnifiedReal.FromDouble(x);
        var otherAsUr = UnifiedReal.FromDouble(other);
        if (x != 0.0)
        {
            Assert.True(
                double.IsInfinity(3.0 / x) || UlpError(3.0 / x, Three / xAsUr) == 0,
                "div 3: " + x);
            Assert.True(
                double.IsInfinity(-17.0 / x) || UlpError(-17.0 / x, Minus17 / xAsUr) == 0,
                "div -17: " + x);
            Assert.True(
                !double.IsFinite(other / x) || UlpError(other / x, otherAsUr / xAsUr) == 0,
                "div " + other + ": " + x);
        }

        var result = Math.Exp(x);
        if (result != 0 && !double.IsInfinity(result))
            Assert.True(UlpError(result, xAsUr.Exp()) <= 1,
                "exp: " + x); // Otherwise the UnifiedReal computation may be intractible.

        if (x > 0) Assert.True(UlpError(Math.Log(x), xAsUr.Ln()) <= 1, "ln: " + x);

        if (x > 0) Assert.True(UlpError(Math.Log10(x), xAsUr.Log()) <= 1, "log10: " + x);

        if (x >= 0)
        {
            var rt = Math.Sqrt(x);
            var urRt = xAsUr.Sqrt();
            Assert.True(UlpError(rt, urRt) == 0, "sqrt: " + x);
        }

        Assert.True(UlpError(Math.Sin(x), xAsUr.Sin()) <= 1, "sin: " + x);
        Assert.True(UlpError(Math.Cos(x), xAsUr.Cos()) <= 1, "cos: " + x);
        Assert.True(UlpError(Math.Tan(x), xAsUr.Tan()) <= 1, "tan: " + x);
        Assert.True(UlpError(Math.Atan(x), xAsUr.Atan()) <= 1, "atan: " + x);
        if (Math.Abs(x) <= 1)
        {
            Assert.True(UlpError(Math.Asin(x), xAsUr.Asin()) <= 1, "asin: " + x);
            Assert.True(UlpError(Math.Acos(x), xAsUr.Acos()) <= 1, "acos: " + x);
        }

        if (double.IsNaN(other)) return;

        var h = Hypot(x, other); // C# Math.Hypot equivalent
        var hUr = Hypot(xAsUr, otherAsUr);
        if (double.IsInfinity(h))
        {
            var h2 = hUr.ToDouble();
            Assert.True(
                double.IsInfinity(h2) ||
                double.IsInfinity(Math.BitIncrement(h2)),
                "inf hypot: " + x + ", " + other + ", hypot = " + h + ", hypot as UR = " + hUr
                + ", hypot from UR = " + h2);
            // TODO: Since h2 is not yet correctly rounded, this could conceivably still fail
            // spuriously. But that's extremely unlikely.
        }
        else
        {
            var error = UlpError(h, hUr);
            Assert.True(error <= 1, "hypot: " + x + ", " + other);
        }

        if (x >= 0.0 || other == Math.Round(other))
        {
            var p = Math.Pow(x, other);
            if (!double.IsInfinity(p))
            {
                var urP = xAsUr.Pow(otherAsUr);
                if (urP.CompareTo(UnifiedReal.FromDouble(p), ComparePrec) != 0)
                    Assert.True(ApproxUlpError(p, urP) <= 1, "pow: " + x + ", " + other);
            }
        }
    }

    private void CheckFunctionsAt(double x)
    {
        CheckFunctionsAt(x, GetRandomDouble());
    }

    [Fact]
    public void CheckUlpError()
    {
        for (var x = -1.0E-300 / 5.0; !double.IsInfinity(x); x *= -1.7)
        {
            // Sign changes every time, but absolute value increases; about 1000 iterations.
            var prev = Math.BitDecrement(x);
            var prevprev = Math.BitDecrement(prev);
            Assert.Equal(0, UlpError(x, UnifiedReal.FromDouble(x)));
            Assert.Equal(2, UlpError(prev, UnifiedReal.FromDouble(x)));
            Assert.Equal(2, UlpError(x, UnifiedReal.FromDouble(prev)));
            Assert.Equal(Wrong, UlpError(prevprev, UnifiedReal.FromDouble(x)));
            Assert.Equal(Wrong, UlpError(x, UnifiedReal.FromDouble(prevprev)));
        }
    }

    [Fact]
    public void AFewSpecialDoubleChecks()
    {
        CheckFunctionsAt(1.7976931348623157E308, -1.0128673137222576E307);
        // Fails for hypot() on OpenJDK: checkFunctionsAt(-2.6718173667255144E-307, -1.1432573432387167E-308);
        CheckFunctionsAt(0.0);
        CheckFunctionsAt(-0.0);
        CheckFunctionsAt(1.0);
        CheckFunctionsAt(-1.0);
        CheckFunctionsAt(0.5);
        CheckFunctionsAt(-0.5);
        CheckFunctionsAt(double.Epsilon);
        CheckFunctionsAt(double.MinValue);
        CheckFunctionsAt(double.MaxValue);
    }

    [Fact]
    public void ManyRandomDoubleChecks()
    {
        var printTimes = true;
        var nIters = 10000;
        var startTime = DateTime.Now;
        for (var i = 0; i < nIters; i++)
        {
            var x = GetRandomDouble();
            CheckFunctionsAt(x);
        }

        var finishTime = DateTime.Now;
        var elapsedTime = finishTime - startTime;
        if (printTimes)
            testOutputHelper.WriteLine(
                $"{nIters} iterations took {elapsedTime.TotalMilliseconds} msecs or {elapsedTime.TotalMilliseconds / nIters} msecs/iter");
        else
            testOutputHelper.WriteLine($"{nIters} iterations");
    }
}