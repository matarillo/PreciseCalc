using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Representation of the negation of a constructive real.
/// </summary>
internal sealed class NegateConstructiveReal(ConstructiveReal x) : ConstructiveReal
{
    private protected override BigInteger Approximate(int precision) => -x.GetApproximation(precision);
}