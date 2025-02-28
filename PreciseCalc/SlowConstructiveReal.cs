using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Represents a constructive real number that evaluates in a way that minimizes reevaluations.
/// This approach is beneficial when increasing precision is expensive.
/// </summary>
/// <remarks>
/// A specialization of ConstructiveReal for cases in which Approximate() calls
/// to increase evaluation precision are somewhat expensive.
/// If we need to (re)evaluate, we speculatively evaluate to slightly
/// higher precision, minimizing reevaluations.
/// Note that this requires any arguments to be evaluated to higher
/// precision than absolutely necessary.  It can thus potentially
/// result in lots of wasted effort, and should be used judiciously.
/// This assumes that the order of magnitude of the number is roughly one.
/// </remarks>
internal abstract class SlowConstructiveReal : ConstructiveReal
{
    private const int MaxPrecision = -64;
    private const int PrecisionIncrement = 32;

    public override BigInteger GetApproximation(int precision)
    {
        CheckPrecision(precision);
        if (ApprValid && precision >= MinPrec)
        {
            return Scale(MaxAppr, MinPrec - precision);
        }
        else
        {
            int evalPrec = (precision >= MaxPrecision)
                ? MaxPrecision
                : (precision - PrecisionIncrement + 1) & ~(PrecisionIncrement - 1);
            BigInteger result = Approximate(evalPrec);
            MinPrec = evalPrec;
            MaxAppr = result;
            ApprValid = true;
            return Scale(result, evalPrec - precision);
        }
    }
}