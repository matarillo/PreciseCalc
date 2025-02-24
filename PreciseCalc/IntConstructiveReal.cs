using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Represents a constructive real number that is exactly an integer.
/// </summary>
/// <param name="n">The integer value to represent.</param>
internal sealed class IntConstructiveReal(BigInteger n) : ConstructiveReal
{
    /// <summary>
    /// Approximates the integer at the specified precision.
    /// Since the number is an exact integer, it is simply scaled appropriately.
    /// </summary>
    /// <param name="precision">The precision at which to approximate the value.</param>
    /// <returns>The approximation of the integer scaled by 2^precision.</returns>
    protected override BigInteger Approximate(int precision) => Scale(n, -precision);
}