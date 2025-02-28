using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Representation of the sum of 2 constructive reals.
/// </summary>
/// <remarks>
/// Args need to be evaluated so that each error is &lt; 1/4 ulp.
/// Rounding error from the cale call is &lt;= 1/2 ulp, so that
/// final error is &lt; 1 ulp.
/// </remarks>
internal class AddConstructiveReal(ConstructiveReal x, ConstructiveReal y) : ConstructiveReal
{
    private protected override BigInteger Approximate(int precision) =>
        Scale(x.GetApproximation(precision - 2) + y.GetApproximation(precision - 2), -2);
}