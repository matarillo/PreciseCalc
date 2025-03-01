using System.Numerics;

namespace PreciseCalc;

/// <summary>
///     Represents the square root of a constructive real number.
///     Uses a combination of Newton iteration and floating-point approximations.
/// </summary>
/// <param name="x">The constructive real number to take the square root of.</param>
internal sealed class SqrtConstructiveReal(ConstructiveReal x) : ConstructiveReal
{
    private const int FpPrec = 50; // Conservative estimate of significant bits in double precision
    private const int FpOpPrec = 60;

    /// <summary>
    ///     Initializes a new instance with an explicitly provided initial approximation.
    /// </summary>
    /// <remarks>
    ///     Explicitly provide an initial approximation.
    ///     Useful for arithmetic geometric mean algorithms, where we've previously computed a very similar square root.
    /// </remarks>
    public SqrtConstructiveReal(ConstructiveReal x, int minPrec, BigInteger maxAppr) : this(x)
    {
        MinPrec = minPrec;
        MaxAppr = maxAppr;
        ApprValid = true;
    }

    /// <summary>
    ///     Approximates the square root at the specified precision.
    ///     Uses Newton iteration for higher precision and floating-point approximation otherwise.
    /// </summary>
    /// <param name="precision">The precision at which to approximate the square root.</param>
    /// <returns>The approximation of sqrt(x) scaled by 2^precision.</returns>
    private protected override BigInteger Approximate(int precision)
    {
        var maxOpPrecNeeded = 2 * precision - 1;
        var msd = x.RefineMsd(maxOpPrecNeeded);
        if (msd <= maxOpPrecNeeded) return BigInteger.Zero;

        var resultMsd = msd / 2; // +- 1
        var resultDigits = resultMsd - precision; // +- 2

        if (resultDigits > FpPrec)
        {
            // Compute less precise approximation and use a Newton iteration.
            var apprDigits = resultDigits / 2 + 6;
            // This should be conservative.  Is fewer enough?
            var apprPrec = resultMsd - apprDigits;
            var prodPrec = 2 * apprPrec;

            // First compute the argument to maximal precision, so we don't end up
            // reevaluating it incrementally.
            var opAppr = x.GetApproximation(prodPrec);
            var lastAppr = GetApproximation(apprPrec);

            // Compute (last_appr * last_appr + op_appr) / last_appr / 2
            // while adjusting the scaling to make everything work
            var prodPrecScaledNumerator = lastAppr * lastAppr + opAppr;
            var scaledNumerator = Scale(prodPrecScaledNumerator, apprPrec - precision);
            var shiftedResult = scaledNumerator / lastAppr;
            return (shiftedResult + BigInteger.One) >> 1;
        }

        // Use a double precision floating point approximation.
        // Make sure all precisions are even
        var opPrec = (msd - FpOpPrec) & ~1;
        var workingPrec = opPrec - FpOpPrec;
        var scaledBiAppr = x.GetApproximation(opPrec) << FpOpPrec;
        var scaledAppr = (double)scaledBiAppr;

        if (scaledAppr < 0.0)
            throw new ArithmeticException("sqrt(negative)");

        var scaledFpSqrt = Math.Sqrt(scaledAppr);
        var scaledSqrt = new BigInteger((long)scaledFpSqrt);
        var shiftCount = workingPrec / 2 - precision;
        return Scale(scaledSqrt, shiftCount);
    }
}