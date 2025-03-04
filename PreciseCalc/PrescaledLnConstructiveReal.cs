using System.Numerics;

namespace PreciseCalc;

/// <summary>
///     Represents ln(1 + x) using a Taylor series expansion. Assumes |x| &lt; 1/2.
/// </summary>
internal class PrescaledLnConstructiveReal(ConstructiveReal x) : SlowConstructiveReal
{
    /// <summary>
    ///     Compute an approximation of ln(1+x) to precision
    ///     prec. This assumes |x| &lt; 1/2.
    ///     It uses a Taylor series expansion.
    /// </summary>
    /// <remarks>
    ///     Unfortunately there appears to be no way to take
    ///     advantage of old information.
    ///     Note: this is known to be a bad algorithm for
    ///     floating point.  Unfortunately, other alternatives
    ///     appear to require precomputed tabular information.
    /// </remarks>
    /// <exception cref="OperationCanceledException"></exception>
    private protected override BigInteger Approximate(int precision)
    {
        if (precision >= 0) return Big0;

        var iterationsNeeded = -precision; // conservative estimate > 0.
        //  Claim: each intermediate term is accurate
        //  to 2*2^calc_precision.  Total error is
        //  2*iterations_needed*2^calc_precision
        //  exclusive of error in op.
        var calcPrecision = precision - BoundLog2(2 * iterationsNeeded) - 4; // for error in op, truncation.
        var opPrec = precision - 3;
        var opAppr = x.GetApproximation(opPrec);
        // Error analysis as for exponential.
        var xNth = Scale(opAppr, opPrec - calcPrecision);
        var currentTerm = xNth;
        var currentSum = currentTerm;
        var n = 1;
        var currentSign = 1; // (-1)^(n-1)
        var maxTruncError = Big1 << (precision - 4 - calcPrecision);

        while (BigInteger.Abs(currentTerm).CompareTo(maxTruncError) >= 0)
        {
            if (PleaseStop) throw new OperationCanceledException();
            n += 1;
            currentSign = -currentSign;
            xNth = Scale(xNth * opAppr, opPrec);
            currentTerm = xNth / (n * currentSign); // x**n / (n * (-1)**(n-1))
            currentSum += currentTerm;
        }

        return Scale(currentSum, calcPrecision - precision);
    }
}