namespace PreciseCalc;

/// <summary>
/// Represents a unary function on constructive reals.
/// </summary>
/// <remarks>
/// The <see cref="Execute"/> method computes the function result.
/// Subclasses should implement specific mathematical functions.
/// </remarks>
public abstract class UnaryCRFunction
{
    /// <summary>
    /// Computes the function result for a given <see cref="ConstructiveReal"/>.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The result of applying the function.</returns>
    public abstract ConstructiveReal Execute(ConstructiveReal x);

    // --- Predefined Functions ---
    /// <summary>
    /// The identity function f(x) = x.
    /// </summary>
    public static readonly UnaryCRFunction IdentityFunction = new IdentityUnaryCRFunction();

    /// <summary>
    /// The negation function f(x) = -x.
    /// </summary>
    public static readonly UnaryCRFunction NegateFunction = new NegateUnaryCRFunction();

    /// <summary>
    /// The inverse function f(x) = 1/x.
    /// </summary>
    public static readonly UnaryCRFunction InverseFunction = new InverseUnaryCRFunction();

    /// <summary>
    /// The absolute value function f(x) = |x|.
    /// </summary>
    public static readonly UnaryCRFunction AbsFunction = new AbsUnaryCRFunction();

    /// <summary>
    /// The exponential function f(x) = exp(x).
    /// </summary>
    public static readonly UnaryCRFunction ExpFunction = new ExpUnaryCRFunction();

    /// <summary>
    /// The cosine function f(x) = cos(x).
    /// </summary>
    public static readonly UnaryCRFunction CosFunction = new CosUnaryCRFunction();

    /// <summary>
    /// The sine function f(x) = sin(x).
    /// </summary>
    public static readonly UnaryCRFunction SinFunction = new SinUnaryCRFunction();

    /// <summary>
    /// The tangent function f(x) = tan(x).
    /// </summary>
    public static readonly UnaryCRFunction TanFunction = new TanUnaryCRFunction();

    /// <summary>
    /// The arcsine function f(x) = asin(x).
    /// </summary>
    public static readonly UnaryCRFunction AsinFunction = new AsinUnaryCRFunction();

    /// <summary>
    /// The arccosine function f(x) = acos(x).
    /// </summary>
    public static readonly UnaryCRFunction AcosFunction = new AcosUnaryCRFunction();

    /// <summary>
    /// The arctangent function f(x) = atan(x).
    /// </summary>
    public static readonly UnaryCRFunction AtanFunction = new AtanUnaryCRFunction();

    /// <summary>
    /// The natural logarithm function f(x) = ln(x).
    /// </summary>
    public static readonly UnaryCRFunction LnFunction = new LnUnaryCRFunction();

    /// <summary>
    /// The square root function f(x) = sqrt(x).
    /// </summary>
    public static readonly UnaryCRFunction SqrtFunction = new SqrtUnaryCRFunction();

    /// <summary>
    /// Composes this function with another function.
    /// </summary>
    /// <param name="f2">The function to compose with.</param>
    /// <returns>A new function representing the composition.</returns>
    public UnaryCRFunction Compose(UnaryCRFunction f2) => new ComposedUnaryCRFunction(this, f2);

    /// <summary>
    /// Computes the inverse of this function on the given interval.
    /// </summary>
    /// <remarks>
    /// Compute the inverse of this function, which must be defined
    /// and strictly monotone on the interval [<paramref name="low"/>, <paramref name="high"/>].
    /// The resulting function is defined only on the image of
    /// [<paramref name="low"/>, <paramref name="high"/>].
    /// The original function may be either increasing or decreasing.
    /// </remarks>
    /// <param name="low">Lower bound of the interval</param>
    /// <param name="high">Upper bound of the interval</param>
    /// <returns>The inverse function as a UnaryCRFunction</returns>
    public UnaryCRFunction InverseMonotone(ConstructiveReal low, ConstructiveReal high) =>
        new InverseMonotoneUnaryCRFunction(this, low, high);

    /// <summary>
    /// Compute the derivative of a function.
    /// The function must be defined on the interval [<paramref name="low"/>, <paramref name="high"/>],
    /// and the derivative must exist, and must be continuous and
    /// monotone in the open interval [<paramref name="low"/>, <paramref name="high"/>].
    /// The result is defined only in the open interval.
    /// </summary>
    /// <param name="low">Lower bound of the interval</param>
    /// <param name="high">Upper bound of the interval</param>
    /// <returns>The derivative function as a UnaryCRFunction</returns>
    public UnaryCRFunction MonotoneDerivative(ConstructiveReal low, ConstructiveReal high) =>
        new MonotoneDerivativeUnaryCRFunction(this, low, high);
}

// --- Implementations of the predefined functions ---
class IdentityUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x;
}

class NegateUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Negate();
}

class InverseUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Inverse();
}

class AbsUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Abs();
}

class ExpUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Exp();
}

class CosUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Cos();
}

class SinUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Sin();
}

class TanUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Sin().Divide(x.Cos());
}

class AsinUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Asin();
}

class AcosUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Acos();
}

class AtanUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x)
    {
        var x2 = x.Multiply(x);
        var absSinAtan = x2.Divide(ConstructiveReal.One.Add(x2)).Sqrt();
        ConstructiveReal sinAtan = x.Select(absSinAtan.Negate(), absSinAtan);
        return sinAtan.Asin();
    }
}

class LnUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Ln();
}

class SqrtUnaryCRFunction : UnaryCRFunction
{
    public override ConstructiveReal Execute(ConstructiveReal x) => x.Sqrt();
}

// --- Function Composition ---
class ComposedUnaryCRFunction : UnaryCRFunction
{
    private readonly UnaryCRFunction f1, f2;

    public ComposedUnaryCRFunction(UnaryCRFunction f1, UnaryCRFunction f2)
    {
        this.f1 = f1;
        this.f2 = f2;
    }

    public override ConstructiveReal Execute(ConstructiveReal x) => f1.Execute(f2.Execute(x));
}

/// <summary>
/// Computes the inverse of a monotone function on the given interval.
/// </summary>
class InverseMonotoneUnaryCRFunction : UnaryCRFunction
{
    private readonly InverseIncreasingConstructiveReal.Data _data;

    /// <summary>
    /// Computes the inverse of a monotone function on the given interval.
    /// </summary>
    public InverseMonotoneUnaryCRFunction(UnaryCRFunction func, ConstructiveReal l, ConstructiveReal h)
    {
        _data = new InverseIncreasingConstructiveReal.Data(func, l, h);
    }

    public override ConstructiveReal Execute(ConstructiveReal x) => new InverseIncreasingConstructiveReal(x, _data);
}

class MonotoneDerivativeUnaryCRFunction : UnaryCRFunction
{
    private readonly MonotoneDerivativeConstructiveReal.Data _data;

    public MonotoneDerivativeUnaryCRFunction(UnaryCRFunction func, ConstructiveReal l, ConstructiveReal h)
    {
        _data = new MonotoneDerivativeConstructiveReal.Data(func, l, h);
    }

    public override ConstructiveReal Execute(ConstructiveReal x) => new MonotoneDerivativeConstructiveReal(x, _data);
}