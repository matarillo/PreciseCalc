using System.Numerics;

namespace PreciseCalc;

/// <summary>
///     Represents a constructive real number assumed to be an integer.
///     This prevents evaluation beyond the decimal point for performance optimization.
/// </summary>
/// <remarks>
///     Representation of a number that may not have been completely
///     evaluated, but is assumed to be an integer.  Hence, we never
///     evaluate beyond the decimal point.
/// </remarks>
internal class AssumedIntConstructiveReal(ConstructiveReal x) : ConstructiveReal
{
    private protected override BigInteger Approximate(int precision)
    {
        return precision >= 0
            ? x.GetApproximation(precision)
            : Scale(x.GetApproximation(0), -precision);
    }
}