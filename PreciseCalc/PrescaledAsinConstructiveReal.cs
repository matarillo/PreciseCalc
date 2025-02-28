using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Represents arcsin(x) using a Taylor series expansion. Assumes |x| &lt; (1/2)^(1/3).
/// </summary>
internal class PrescaledAsinConstructiveReal(ConstructiveReal x) : SlowConstructiveReal
{
    private protected override BigInteger Approximate(int precision)
    {
        // The Taylor series is the sum of x^(2n+1) * (2n)!/(4^n n!^2 (2n+1))
        // Note that (2n)!/(4^n n!^2) is always less than one.
        // (The denominator is effectively 2n*2n*(2n-2)*(2n-2)*...*2*2
        // which is clearly > (2n)!)
        // Thus, all terms are bounded by x^(2n+1).
        // Unfortunately, there's no easy way to prescale the argument
        // to less than 1/sqrt(2), and we can only approximate that.
        // Thus, the worst case iteration count is fairly high.
        // But it doesn't make much difference.
        if (precision >= 2) return Big0; // Never bigger than 4.

        int iterationsNeeded = -3 * precision / 2 + 4;
        // conservative estimate > 0.
        // Follows from assumed bound on x and
        // the fact that only every other Taylor
        // Series term is present.

        //  Claim: each intermediate term is accurate
        //  to 2*2^calc_precision.
        //  Total rounding error in series computation is
        //  2*iterations_needed*2^calc_precision,
        //  exclusive of error in op.
        int calcPrecision = precision - BoundLog2(2 * iterationsNeeded) - 4; // for error in op, truncation.
        int opPrec = precision - 3; // always <= -2
        BigInteger opAppr = x.GetApproximation(opPrec);
        // Error in argument results in error of < 1/4 ulp.
        // (Derivative is bounded by 2 in the specified range, and we use
        // 3 extra digits.)
        // Ignoring the argument error, each term has an error of
        // < 3ulps relative to calc_precision, which is more precise than p.
        // Cumulative arithmetic rounding error is < 3/16 ulp (relative to p).
        // Series truncation error < 2/16 ulp.  (Each computed term
        // is at most 2/3 of last one, so some of remaining series <
        // 3/2 * current term.)
        // Final rounding error is <= 1/2 ulp.
        // Thus, final error is < 1 ulp (relative to p).
        BigInteger maxLastTerm = Big1 << (precision - 4 - calcPrecision);
        int exp = 1; // Current exponent, = 2n+1 in above expression
        BigInteger currentTerm = opAppr << (opPrec - calcPrecision);
        BigInteger currentSum = currentTerm;
        BigInteger currentFactor = currentTerm;
        // Current scaled Taylor series term
        // before division by the exponent.
        // Accurate to 3 ulp at calc_precision.
        while (BigInteger.Abs(currentTerm).CompareTo(maxLastTerm) >= 0)
        {
            if (PleaseStop) throw new OperationCanceledException();
            exp += 2;
            // current_factor = current_factor * op * op * (exp-1) * (exp-2) /
            // (exp-1) * (exp-1), with the two exp-1 factors cancelling,
            // giving
            // current_factor = current_factor * op * op * (exp-2) / (exp-1)
            // Thus the error any in the previous term is multiplied by
            // op^2, adding an error of < (1/2)^(2/3) < 2/3 the original
            // error.
            currentFactor *= exp - 2;
            currentFactor = Scale(currentFactor * opAppr, opPrec + 2);
            // Carry 2 extra bits of precision forward; thus
            // this effectively introduces 1/8 ulp error.
            currentFactor *= opAppr;
            currentFactor /= exp - 1; // Another 1/4 ulp error here.
            currentFactor = Scale(currentFactor, opPrec - 2); // Remove extra 2 bits.  1/2 ulp rounding error.
            // Current_factor has original 3 ulp rounding error, which we
            // reduced by 1, plus < 1 ulp new rounding error.
            currentTerm = currentFactor / exp;
            // Contributes 1 ulp error to sum plus at most 3 ulp
            // from current_factor.
            currentSum += currentTerm;
        }

        return Scale(currentSum, calcPrecision - precision);
    }
}