using System.Numerics;

namespace PreciseCalc;

/// <summary>
///     Representation of the selection of a constructive real.
/// </summary>
/// <remarks>
///     x     if selector &lt; 0<br />
///     y     if selector ^gt;= 0<br />
///     Assumes x = y if selector = 0
/// </remarks>
internal class SelectConstructiveReal : ConstructiveReal
{
    private readonly ConstructiveReal _op1;
    private readonly ConstructiveReal _op2;
    private readonly ConstructiveReal _selector;
    private int _selectorSign;

    public SelectConstructiveReal(ConstructiveReal selector, ConstructiveReal x, ConstructiveReal y)
    {
        _selector = selector;
        _selectorSign = _selector.GetApproximation(-20).Sign;
        _op1 = x;
        _op2 = y;
    }

    private protected override BigInteger Approximate(int precision)
    {
        if (_selectorSign < 0) return _op1.GetApproximation(precision);
        if (_selectorSign > 0) return _op2.GetApproximation(precision);

        var op1Appr = _op1.GetApproximation(precision - 1);
        var op2Appr = _op2.GetApproximation(precision - 1);
        var diff = BigInteger.Abs(op1Appr - op2Appr);

        if (diff.CompareTo(Big1) <= 0)
            // close enough; use either
            return Scale(op1Appr, -1);

        // op1 and op2 are different; selector != 0;
        // safe to get sign of selector.
        _selectorSign = _selector.Sign();
        return _selectorSign < 0 ? Scale(op1Appr, -1) : Scale(op2Appr, -1);
    }
}