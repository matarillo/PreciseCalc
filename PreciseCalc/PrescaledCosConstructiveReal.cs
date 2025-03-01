using System.Numerics;

namespace PreciseCalc;

/// <summary>
///     Represents the cosine of a constructive real number using a Taylor series expansion.
///     Assumes |x| &lt; 1.
/// </summary>
internal class PrescaledCosConstructiveReal(ConstructiveReal x) : SlowConstructiveReal
{
    private protected override BigInteger Approximate(int precision)
    {
        if (precision >= 1) return Big0;

        var iterationsNeeded = -precision / 2 + 4;
        //  Claim: each intermediate term is accurate
        //  to 2*2^calc_precision.
        //  Total rounding error in series computation is
        //  2*iterations_needed*2^calc_precision,
        //  exclusive of error in op.
        var calcPrecision = precision - BoundLog2(2 * iterationsNeeded) - 4; // for error in op, truncation.
        var opPrec = precision - 2;
        var opAppr = x.GetApproximation(opPrec);
        // Error in argument results in error of < 1/4 ulp.
        // Cumulative arithmetic rounding error is < 1/16 ulp.
        // Series truncation error < 1/16 ulp.
        // Final rounding error is <= 1/2 ulp.
        // Thus, final error is < 1 ulp.
        var maxTruncError = Big1 << (precision - 4 - calcPrecision);
        var n = 0;
        var currentTerm = Big1 << -calcPrecision;
        var currentSum = currentTerm;

        while (BigInteger.Abs(currentTerm).CompareTo(maxTruncError) >= 0)
        {
            if (PleaseStop) throw new OperationCanceledException();
            n += 2;
            // current_term = - current_term * op * op / n * (n - 1)
            currentTerm = Scale(currentTerm * opAppr, opPrec);
            currentTerm = Scale(currentTerm * opAppr, opPrec);
            var divisor = new BigInteger(-n) * (n - 1);
            currentTerm /= divisor;
            currentSum += currentTerm;
        }

        return Scale(currentSum, calcPrecision - precision);
    }
}