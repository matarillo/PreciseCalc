using System.Numerics;

namespace PreciseCalc;

/// <summary>
///     Computes the derivative of a monotone function using numerical approximation.
/// </summary>
internal class MonotoneDerivativeConstructiveReal : ConstructiveReal
{
    private readonly ConstructiveReal _arg;
    private readonly Data _data;
    private readonly ConstructiveReal _fArg;
    private readonly int _maxDeltaMsd;

    public MonotoneDerivativeConstructiveReal(ConstructiveReal x, Data data)
    {
        _data = data;
        _arg = x;
        _fArg = data.F.Execute(x);

        // The following must converge, since arg must be in the open interval
        var leftDiff = _arg - data.Low;
        var rightDiff = data.High - _arg;
        var maxDeltaLeftMsd = leftDiff.GetMsd();
        var maxDeltaRightMsd = rightDiff.GetMsd();

        if (leftDiff.Sign() < 0 || rightDiff.Sign() < 0) throw new ArithmeticException("Function is not monotone");

        _maxDeltaMsd = Math.Min(maxDeltaLeftMsd, maxDeltaRightMsd);
    }

    private protected override BigInteger Approximate(int precision)
    {
        const int extraPrec = 4;
        var logDelta = precision - _data.Deriv2Msd;

        // Ensure that we stay within the interval
        if (logDelta > _maxDeltaMsd) logDelta = _maxDeltaMsd;

        logDelta -= extraPrec;

        var delta = One << logDelta;

        var left = _arg - delta;
        var right = _arg + delta;
        var fLeft = _data.F.Execute(left);
        var fRight = _data.F.Execute(right);

        var leftDeriv = (_fArg - fLeft) >> logDelta;
        var rightDeriv = (fRight - _fArg) >> logDelta;

        var evalPrec = precision - extraPrec;
        var apprLeftDeriv = leftDeriv.GetApproximation(evalPrec);
        var apprRightDeriv = rightDeriv.GetApproximation(evalPrec);
        var derivDifference = BigInteger.Abs(apprRightDeriv - apprLeftDeriv);

        if (derivDifference < Big8) return Scale(apprLeftDeriv, -extraPrec);

        if (PleaseStop) throw new OperationCanceledException("Calculation was cancelled");

        // Update derivative bound and try again
        _data.Deriv2Msd = evalPrec + (int)derivDifference.GetBitLength() + 4 /* slop */ - logDelta;
        return Approximate(precision);
    }

    /// <summary>
    ///     Contains the data needed for derivative computation.
    /// </summary>
    internal class Data
    {
        internal readonly int DifferenceMsd;

        // Monotone increasing. If it was monotone decreasing, we negate it.
        internal readonly UnaryCRFunction F;
        internal readonly ConstructiveReal FHigh;

        // Corresponding function values.
        internal readonly ConstructiveReal FLow;
        internal readonly ConstructiveReal FMid;
        internal readonly ConstructiveReal High;

        // endpoints and midpoint of interval
        internal readonly ConstructiveReal Low;
        internal readonly ConstructiveReal Mid;

        // Rough approx. of msd of second derivative.
        // This is increased to be an appr. bound on the msd of |(f'(y)-f'(x))/(x-y)|
        // for any pair of points x and y we have considered.
        // It may be better to keep a copy per derivative value.
        internal int Deriv2Msd; // Not readonly because it may be updated during computation

        internal Data(UnaryCRFunction func, ConstructiveReal l, ConstructiveReal h)
        {
            F = func;
            Low = l;
            High = h;
            Mid = (l + h) >> 1;

            FLow = func.Execute(l);
            FMid = func.Execute(Mid);
            FHigh = func.Execute(h);

            var difference = h - l;

            // Compute approximate msd of ((f_high - f_mid) - (f_mid - f_low))/(high - low)
            // This should be a very rough approximation to the second derivative.
            // We add a little slop to err on the high side, since a low estimate will cause extra iterations.
            var apprDiff2 = FHigh - (FMid << 1) + FLow;
            DifferenceMsd = difference.GetMsd();
            Deriv2Msd = apprDiff2.GetMsd() - DifferenceMsd + 4;
        }
    }
}