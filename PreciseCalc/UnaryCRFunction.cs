namespace PreciseCalc;

/// <summary>
///     Represents a unary function on constructive reals.
/// </summary>
/// <remarks>
///     The <see cref="Execute" /> method computes the function result.
///     Subclasses should implement specific mathematical functions.
/// </remarks>
public abstract class UnaryCRFunction
{
    // --- Predefined Functions ---
    /// <summary>
    ///     The identity function f(x) = x.
    /// </summary>
    public static readonly UnaryCRFunction IdentityFunction = new IdentityUnaryCRFunction();

    /// <summary>
    ///     The negation function f(x) = -x.
    /// </summary>
    public static readonly UnaryCRFunction NegateFunction = new NegateUnaryCRFunction();

    /// <summary>
    ///     The inverse function f(x) = 1/x.
    /// </summary>
    public static readonly UnaryCRFunction InverseFunction = new InverseUnaryCRFunction();

    /// <summary>
    ///     The absolute value function f(x) = |x|.
    /// </summary>
    public static readonly UnaryCRFunction AbsFunction = new AbsUnaryCRFunction();

    /// <summary>
    ///     The exponential function f(x) = exp(x).
    /// </summary>
    public static readonly UnaryCRFunction ExpFunction = new ExpUnaryCRFunction();

    /// <summary>
    ///     The cosine function f(x) = cos(x).
    /// </summary>
    public static readonly UnaryCRFunction CosFunction = new CosUnaryCRFunction();

    /// <summary>
    ///     The sine function f(x) = sin(x).
    /// </summary>
    public static readonly UnaryCRFunction SinFunction = new SinUnaryCRFunction();

    /// <summary>
    ///     The tangent function f(x) = tan(x).
    /// </summary>
    public static readonly UnaryCRFunction TanFunction = new TanUnaryCRFunction();

    /// <summary>
    ///     The arcsine function f(x) = asin(x).
    /// </summary>
    public static readonly UnaryCRFunction AsinFunction = new AsinUnaryCRFunction();

    /// <summary>
    ///     The arccosine function f(x) = acos(x).
    /// </summary>
    public static readonly UnaryCRFunction AcosFunction = new AcosUnaryCRFunction();

    /// <summary>
    ///     The arctangent function f(x) = atan(x).
    /// </summary>
    public static readonly UnaryCRFunction AtanFunction = new AtanUnaryCRFunction();

    /// <summary>
    ///     The natural logarithm function f(x) = ln(x).
    /// </summary>
    public static readonly UnaryCRFunction LnFunction = new LnUnaryCRFunction();

    /// <summary>
    ///     The square root function f(x) = sqrt(x).
    /// </summary>
    public static readonly UnaryCRFunction SqrtFunction = new SqrtUnaryCRFunction();

    /// <summary>
    ///     Computes the function result for a given <see cref="ConstructiveReal" />.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The result of applying the function.</returns>
    public abstract ConstructiveReal Execute(ConstructiveReal x);

    /// <summary>
    ///     Composes this function with another function.
    /// </summary>
    /// <param name="f2">The function to compose with.</param>
    /// <returns>A new function representing the composition.</returns>
    public UnaryCRFunction Compose(UnaryCRFunction f2)
    {
        return new ComposedUnaryCRFunction(this, f2);
    }

    /// <summary>
    ///     Computes the inverse of this function on the given interval.
    /// </summary>
    /// <remarks>
    ///     Compute the inverse of this function, which must be defined
    ///     and strictly monotone on the interval [<paramref name="low" />, <paramref name="high" />].
    ///     The resulting function is defined only on the image of
    ///     [<paramref name="low" />, <paramref name="high" />].
    ///     The original function may be either increasing or decreasing.
    /// </remarks>
    /// <param name="low">Lower bound of the interval</param>
    /// <param name="high">Upper bound of the interval</param>
    /// <returns>The inverse function as a UnaryCRFunction</returns>
    public UnaryCRFunction InverseMonotone(ConstructiveReal low, ConstructiveReal high)
    {
        return new InverseMonotoneUnaryCRFunction(this, low, high);
    }

    /// <summary>
    ///     Compute the derivative of a function.
    ///     The function must be defined on the interval [<paramref name="low" />, <paramref name="high" />],
    ///     and the derivative must exist, and must be continuous and
    ///     monotone in the open interval [<paramref name="low" />, <paramref name="high" />].
    ///     The result is defined only in the open interval.
    /// </summary>
    /// <param name="low">Lower bound of the interval</param>
    /// <param name="high">Upper bound of the interval</param>
    /// <returns>The derivative function as a UnaryCRFunction</returns>
    public UnaryCRFunction MonotoneDerivative(ConstructiveReal low, ConstructiveReal high)
    {
        return new MonotoneDerivativeUnaryCRFunction(this, low, high);
    }
}

// --- Implementations of the predefined functions ---
internal class IdentityUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x;
    }
}

internal class NegateUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return -x;
    }
}

internal class InverseUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Inverse();
    }
}

internal class AbsUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Abs();
    }
}

internal class ExpUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Exp();
    }
}

internal class CosUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Cos();
    }
}

internal class SinUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Sin();
    }
}

internal class TanUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Sin() / x.Cos();
    }
}

internal class AsinUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Asin();
    }
}

internal class AcosUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Acos();
    }
}

internal class AtanUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        var x2 = x * x;
        var absSinAtan = (x2 / (ConstructiveReal.One + x2)).Sqrt();
        var sinAtan = x.Select(-absSinAtan, absSinAtan);
        return sinAtan.Asin();
    }
}

internal class LnUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Ln();
    }
}

internal class SqrtUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return x.Sqrt();
    }
}

// --- Function Composition ---
internal class ComposedUnaryCRFunction : UnaryCRFunction
{
    private readonly UnaryCRFunction _f1, _f2;

    public ComposedUnaryCRFunction(UnaryCRFunction f1, UnaryCRFunction f2)
    {
        _f1 = f1;
        _f2 = f2;
    }

    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return _f1.Execute(_f2.Execute(x));
    }
}

/// <summary>
///     Computes the inverse of a monotone function on the given interval.
/// </summary>
internal class InverseMonotoneUnaryCRFunction : UnaryCRFunction
{
    private readonly InverseIncreasingConstructiveReal.Data _data;

    /// <summary>
    ///     Computes the inverse of a monotone function on the given interval.
    /// </summary>
    public InverseMonotoneUnaryCRFunction(UnaryCRFunction func, ConstructiveReal l, ConstructiveReal h)
    {
        _data = new InverseIncreasingConstructiveReal.Data(func, l, h);
    }

    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return new InverseIncreasingConstructiveReal(x, _data);
    }
}

internal class MonotoneDerivativeUnaryCRFunction : UnaryCRFunction
{
    private readonly MonotoneDerivativeConstructiveReal.Data _data;

    public MonotoneDerivativeUnaryCRFunction(UnaryCRFunction func, ConstructiveReal l, ConstructiveReal h)
    {
        _data = new MonotoneDerivativeConstructiveReal.Data(func, l, h);
    }

    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        return new MonotoneDerivativeConstructiveReal(x, _data);
    }
}