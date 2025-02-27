using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Representation of the product of 2 constructive reals.
/// </summary>
internal sealed class MultiplyConstructiveReal : ConstructiveReal
{
    private ConstructiveReal _op1;
    private ConstructiveReal _op2;

    public MultiplyConstructiveReal(ConstructiveReal x, ConstructiveReal y)
    {
        _op1 = x;
        _op2 = y;
    }

    private protected override BigInteger Approximate(int precision)
    {
        int halfPrec = (precision >> 1) - 1;
        int msdOp1 = _op1.GetMsd(halfPrec);
        int msdOp2;

        if (msdOp1 == int.MinValue)
        {
            msdOp2 = _op2.GetMsd(halfPrec);
            if (msdOp2 == int.MinValue)
            {
                // Product is small enough that zero will do as an
                // approximation.
                return Big0;
            }
            else
            {
                // Swap them, so the larger operand (in absolute value)
                // is first.
                (_op1, _op2) = (_op2, _op1);
                msdOp1 = msdOp2;
            }
        }

        // msd_op1 is valid at this point.
        int prec2 = precision - msdOp1 - 3; // Precision needed for op2.
        // The appr. error is multiplied by at most
        // 2 ** (msd_op1 + 1)
        // Thus each approximation contributes 1/4 ulp
        // to the rounding error, and the final rounding adds
        // another 1/2 ulp.
        BigInteger appr2 = _op2.GetApproximation(prec2);
        if (appr2.Sign == 0) return Big0;

        msdOp2 = _op2.GetKnownMsd();
        int prec1 = precision - msdOp2 - 3; // Precision needed for op1.
        BigInteger appr1 = _op1.GetApproximation(prec1);

        int scaleDigits = prec1 + prec2 - precision;
        return Scale(appr1 * appr2, scaleDigits);
    }
}