using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Represents the exponential of a constructive real number using a Taylor series expansion.
/// Assumes |x| &lt; 1/2.
/// </summary>
/// <remarks>
/// Note: this is known to be a bad algorithm for
/// floating point.  Unfortunately, other alternatives
/// appear to require precomputed information.
/// </remarks>
internal class PrescaledExpConstructiveReal(ConstructiveReal x) : ConstructiveReal
{
    private protected override BigInteger Approximate(int precision)
    {
        if (precision >= 1) return Big0;

        int iterationsNeeded = -precision / 2 + 2;
        //  Claim: each intermediate term is accurate
        //  to 2*2^calc_precision.
        //  Total rounding error in series computation is
        //  2*iterations_needed*2^calc_precision,
        //  exclusive of error in op.
        int calcPrecision = precision - BoundLog2(2 * iterationsNeeded) - 4;
        int opPrec = precision - 3;
        BigInteger opAppr = x.GetApproximation(opPrec);
        // Error in argument results in error of < 3/8 ulp.
        // Sum of term eval. rounding error is < 1/16 ulp.
        // Series truncation error < 1/16 ulp.
        // Final rounding error is <= 1/2 ulp.
        // Thus final error is < 1 ulp.
        BigInteger scaledOne = Big1 << -calcPrecision;
        BigInteger currentTerm = scaledOne;
        BigInteger currentSum = scaledOne;
        int n = 0;
        BigInteger maxTruncError = Big1 << (precision - 4 - calcPrecision);

        while (BigInteger.Abs(currentTerm).CompareTo(maxTruncError) >= 0)
        {
            if (PleaseStop) throw new OperationCanceledException();
            n += 1;
            // current_term = current_term * op / n
            currentTerm = Scale(currentTerm * opAppr, opPrec) / n;
            currentSum += currentTerm;
        }

        return Scale(currentSum, calcPrecision - precision);
    }
}