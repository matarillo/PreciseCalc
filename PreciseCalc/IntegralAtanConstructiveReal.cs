using System.Numerics;

namespace PreciseCalc;

/// <summary>
///     Represents the arctangent of 1/n for a small integer n, used for computing PI.
/// </summary>
/// <remarks>
///     The constructive real atan(1/n), where n is a small integer &gt; base.
///     This gives a simple and moderately fast way to compute PI.
/// </remarks>
internal class IntegralAtanConstructiveReal(int x) : SlowConstructiveReal
{
    private protected override BigInteger Approximate(int precision)
    {
        if (precision >= 1) return Big0;

        var iterationsNeeded = -precision / 2 + 2; // conservative estimate > 0.
        //  Claim: each intermediate term is accurate
        //  to 2*base^calc_precision.
        //  Total rounding error in series computation is
        //  2*iterations_needed*base^calc_precision,
        //  exclusive of error in op.
        var calcPrecision = precision - BoundLog2(2 * iterationsNeeded) - 2; // for error in op, truncation.
        // Error in argument results in error of < 3/8 ulp.
        // Cumulative arithmetic rounding error is < 1/4 ulp.
        // Series truncation error < 1/4 ulp.
        // Final rounding error is <= 1/2 ulp.
        // Thus, final error is < 1 ulp.
        var scaledOne = Big1 << -calcPrecision;
        var bigOp = new BigInteger(x);
        var bigOpSquared = bigOp * bigOp;
        var opInverse = scaledOne / bigOp;
        var currentPower = opInverse;
        var currentTerm = opInverse;
        var currentSum = opInverse;
        var currentSign = 1;
        var n = 1;
        var maxTruncError = Big1 << (precision - 2 - calcPrecision);

        while (BigInteger.Abs(currentTerm).CompareTo(maxTruncError) >= 0)
        {
            if (PleaseStop) throw new OperationCanceledException();
            n += 2;
            currentPower /= bigOpSquared;
            currentSign = -currentSign;
            currentTerm = currentPower / (currentSign * n);
            currentSum += currentTerm;
        }

        return Scale(currentSum, calcPrecision - precision);
    }
}