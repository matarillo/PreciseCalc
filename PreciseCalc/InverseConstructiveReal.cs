using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Represents the multiplicative inverse of a constructive real number.
/// Uses Newton iteration to refine estimates.
/// </summary>
/// <param name="x">The constructive real to invert.</param>
internal sealed class InverseConstructiveReal(ConstructiveReal x) : ConstructiveReal
{
    /// <summary>
    /// Approximates the reciprocal (1/x) at the specified precision.
    /// </summary>
    /// <param name="precision">The precision at which to approximate the value.</param>
    /// <returns>The approximation of 1/x.</returns>
    private protected override BigInteger Approximate(int precision)
    {
        int msd = x.GetMsd();
        int invMsd = 1 - msd;
        int digitsNeeded = invMsd - precision + 3;
        // Number of SIGNIFICANT digits needed for argument, excl. msd position, which may
        // be fictitious, since msd routine can be off by 1.  Roughly 1 extra digit is
        // needed since the relative error is the same in the argument and result, but
        // this isn't quite the same as the number of significant digits.  Another digit
        // is needed to compensate for slop in the calculation.
        // One further bit is required, since the final rounding introduces a 0.5 ulp
        // error.        
        int precNeeded = msd - digitsNeeded;
        int logScaleFactor = -precision - precNeeded;

        if (logScaleFactor < 0) return BigInteger.Zero;

        BigInteger dividend = BigInteger.One << logScaleFactor;
        BigInteger scaledDivisor = x.GetApproximation(precNeeded);
        BigInteger absScaledDivisor = BigInteger.Abs(scaledDivisor);
        BigInteger adjDividend = dividend + (absScaledDivisor >> 1); // Adjustment so that final result is rounded.

        BigInteger result = adjDividend / absScaledDivisor;
        return scaledDivisor.Sign < 0 ? -result : result;
    }
}