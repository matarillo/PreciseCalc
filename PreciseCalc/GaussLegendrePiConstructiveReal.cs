using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// The constant PI, computed using the Gauss-Legendre alternating arithmetic-geometric mean algorithm
/// </summary>
/// <remarks>
/// a[0] = 1<br/>
/// b[0] = 1/sqrt(2)<br/>
/// t[0] = 1/4<br/>
/// p[0] = 1<br/>
/// <br/>
/// a[n+1] = (a[n] + b[n])/2        (arithmetic mean, between 0.8 and 1)<br/>
/// b[n+1] = sqrt(a[n] * b[n])      (geometric mean, between 0.7 and 1)<br/>
/// t[n+1] = t[n] - (2^n)(a[n]-a[n+1])^2,  (always between 0.2 and 0.25)<br/>
/// pi is then approximated as (a[n+1]+b[n+1])^2 / 4*t[n+1].
/// </remarks>
internal class GaussLegendrePiConstructiveReal : SlowConstructiveReal
{
    // In addition to the best approximation kept by the CR base class, we keep
    // the entire sequence b[n], to the extent we've needed it so far.  Each
    // reevaluation leads to slightly different sqrt arguments, but the
    // previous result can be used to avoid repeating low precision Newton
    // iterations for the sqrt approximation.

    private static readonly BigInteger Tolerance = new BigInteger(4);
    private static readonly ConstructiveReal SqrtHalf = new SqrtConstructiveReal(One.ShiftRight(1));

    private readonly List<int> _bPrec = new();
    private readonly List<BigInteger> _bVal = new();

    public GaussLegendrePiConstructiveReal()
    {
        _bPrec.Add(0); // Zeroth entry unused.
        _bVal.Add(default);
    }

    private protected override BigInteger Approximate(int precision)
    {
        // Get us back into a consistent state if the last computation
        // was interrupted after pushing onto b_prec.
        if (_bPrec.Count > _bVal.Count)
        {
            _bPrec.RemoveAt(_bPrec.Count - 1);
        }

        // Rough approximations are easy.
        if (precision >= 0) return Scale(Big3, -precision);
        // We need roughly log2(p) iterations.  Each iteration should
        // contribute no more than 2 ulps to the error in the corresponding
        // term (a[n], b[n], or t[n]).  Thus 2log2(n) bits plus a few for the
        // final calulation and rounding suffice.
        int extraEvalPrec = (int)Math.Ceiling(Math.Log(-precision) / Math.Log(2)) + 10;
        // All our terms are implicitly scaled by eval_prec.
        int evalPrec = precision - extraEvalPrec;
        BigInteger a = BigInteger.One << -evalPrec;
        BigInteger b = SqrtHalf.GetApproximation(evalPrec);
        BigInteger t = BigInteger.One << (-evalPrec - 2);
        int n = 0;

        while (a - b - Tolerance > 0)
        {
            // Current values correspond to n, next_ values to n + 1
            // b_prec.size() == b_val.size() >= n + 1
            BigInteger nextA = (a + b) >> 1;
            BigInteger nextB;
            BigInteger aDiff = a - nextA;
            BigInteger bProd = (a * b) >> -evalPrec;
            // We compute square root approximations using a nested
            // temporary CR computation, to avoid implementing BigInteger
            // square roots separately.
            ConstructiveReal bProdAsCr = FromBigInteger(bProd).ShiftRight(-evalPrec);

            if (_bPrec.Count == n + 1)
            {
                // Add an n+1st slot.
                // Take care to make this exception-safe; b_prec and b_val
                // must remain consistent, even if we are interrupted, or run
                // out of memory. It's OK to just push on b_prec in that case.
                ConstructiveReal nextBAsCr = bProdAsCr.Sqrt();
                nextB = nextBAsCr.GetApproximation(evalPrec);
                BigInteger scaledNextB = Scale(nextB, -extraEvalPrec);
                _bPrec.Add(precision);
                _bVal.Add(scaledNextB);
            }
            else
            {
                // Reuse previous approximation to reduce sqrt iterations,
                // hopefully to one.
                ConstructiveReal nextBAsCr = new SqrtConstructiveReal(bProdAsCr, _bPrec[n + 1], _bVal[n + 1]);
                nextB = nextBAsCr.GetApproximation(evalPrec);
                // We assume that set() doesn't throw for any reason.
                _bPrec[n + 1] = precision;
                _bVal[n + 1] = Scale(nextB, -extraEvalPrec);
            }

            // b_prec.size() == b_val.size() >= n + 2
            BigInteger nextT = t - (aDiff * aDiff << (n + evalPrec));
            a = nextA;
            b = nextB;
            t = nextT;
            n++;
        }

        BigInteger sum = a + b;
        BigInteger result = sum * sum / (t << 2);
        return Scale(result, -extraEvalPrec);
    }
}