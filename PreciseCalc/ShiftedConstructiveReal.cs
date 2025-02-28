using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Represents a constructive real number that is multiplied by 2^shift.
/// </summary>
/// <param name="x">The base constructive real number.</param>
/// <param name="n">The power of two by which to scale the value.</param>
internal sealed class ShiftedConstructiveReal(ConstructiveReal x, int n) : ConstructiveReal
{
    /// <summary>
    /// Approximates the shifted value at the specified precision.
    /// </summary>
    /// <param name="precision">The precision at which to approximate the value.</param>
    /// <returns>The approximation of the shifted value.</returns>
    private protected override BigInteger Approximate(int precision) => x.GetApproximation(precision - n);
}