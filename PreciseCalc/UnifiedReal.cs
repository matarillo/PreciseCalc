using System.Diagnostics;
using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Computable real numbers, represented so that we can get exact decidable comparisons for a number
/// of interesting special cases, including rational computations.
/// 
/// A real number is represented as the product of two numbers with different representations:
/// (A) A BoundedRational that can only represent a subset of the rationals, but supports exact
/// computable comparisons. (B) A lazily evaluated "constructive real number" that provides
/// operations to evaluate itself to any requested number of digits. Whenever possible, we choose
/// (B) to be such that we can describe its exact symbolic value using a CRProperty.
/// </summary>
/// <remarks>
/// UnifiedReals, as well as their three components (the rational part, the constructive real part,
/// and the property describing the constructive real part) are logically immutable. (The ConstructiveReal
/// component physically contains an evaluation cache, which is transparently mutated.)
/// 
/// Arithmetic operations and operations that produce finite approximations may throw unchecked
/// exceptions produced by the underlying ConstructiveReal and BoundedRational packages, including
/// PrecisionOverflowException and AbortedException.
/// </remarks>
public class UnifiedReal
{
    #region Default comparison tolerances, in bits

    // Default comparison tolerances, in bits
    private static readonly int DefaultInitialTolerance = -100; // For rough comparison.
    private static readonly int DefaultRelativeTolerance = -1000; // Used only in isComparable.

    private static readonly int DefaultComparisonTolerance = -3500; // Absolute tolerance.

    // Roughly the number of leading zeroes we're willing to accept in comparisons.
    private static readonly int ZeroComparisonTolerance = -5000; // Absolute tolerance.

    #endregion

    #region Other helpful constants

    // Other helpful constants
    private static readonly BigInteger BigTwo = new(2);
    private static readonly BigInteger BigThree = new(3);
    private static readonly BigInteger BigFive = new(5);
    private static readonly BigInteger Big24 = new(24);
    private static readonly BigInteger Big180 = new(180);
    private static readonly BigInteger BigMinusOne = new(-1);
    private static readonly ConstructiveReal CrTwo = ConstructiveReal.FromInt(2);
    private static readonly ConstructiveReal CrThree = ConstructiveReal.FromInt(3);
    private static readonly ConstructiveReal CrTen = ConstructiveReal.FromInt(10);
    private static readonly ConstructiveReal CrLn10 = CrTen.Ln();
    private static readonly BoundedRational Br32 = new(3, 2);
    private static readonly BoundedRational Br180 = new(Big180);

    // Well-known CR constants we try to use in the crFactor position:
    private static readonly ConstructiveReal CrOne = ConstructiveReal.One;
    private static readonly ConstructiveReal CrPI = ConstructiveReal.PI;
    private static readonly ConstructiveReal CrExp = ConstructiveReal.One.Exp();
    private static readonly ConstructiveReal CrSqrt2 = CrTwo.Sqrt();
    private static readonly ConstructiveReal CrSqrt3 = CrThree.Sqrt();

    // Some convenient UnifiedReal constants for our clients.

    /// <summary>
    /// The number pi.
    /// </summary>
    public static readonly UnifiedReal PI = new(CrPI);

    /// <summary>
    /// The number e.
    /// </summary>
    public static readonly UnifiedReal E = new(CrExp);

    /// <summary>
    /// The number 0.
    /// </summary>
    public static readonly UnifiedReal Zero = new(BoundedRational.Zero);

    /// <summary>
    /// The number 1.
    /// </summary>
    public static readonly UnifiedReal One = new(BoundedRational.One);

    /// <summary>
    /// The number -1.
    /// </summary>
    public static readonly UnifiedReal MinusOne = new(BoundedRational.MinusOne);

    /// <summary>
    /// The number 2.
    /// </summary>
    public static readonly UnifiedReal Two = new(BoundedRational.Two);

    /// <summary>
    /// The number 1/2.
    /// </summary>
    public static readonly UnifiedReal Half = new(BoundedRational.Half);

    /// <summary>
    /// The number -1/2.
    /// </summary>
    public static readonly UnifiedReal MinusHalf = new(BoundedRational.MinusHalf);

    /// <summary>
    /// The number 10.
    /// </summary>
    public static readonly UnifiedReal Ten = new(BoundedRational.Ten);

    /// <summary>
    /// The number pi/180.
    /// </summary>
    public static readonly UnifiedReal RadiansPerDegree = new(BoundedRational.Inverse(Br180), CrPI);

    /// <summary>
    /// The square root of 2.
    /// </summary>
    public static readonly UnifiedReal Ln10 = new(CrLn10);

    // Some more that we use internally.
    private static readonly UnifiedReal HalfSqrt2 = new(BoundedRational.Half, CrSqrt2);
    private static readonly UnifiedReal Sqrt3 = new(CrSqrt3);
    private static readonly UnifiedReal HalfSqrt3 = new(BoundedRational.Half, CrSqrt3);
    private static readonly UnifiedReal ThirdSqrt3 = new(BoundedRational.Third, CrSqrt3);
    private static readonly UnifiedReal PIOver2 = new(BoundedRational.Half, CrPI);
    private static readonly UnifiedReal PIOver3 = new(BoundedRational.Third, CrPI);
    private static readonly UnifiedReal PIOver4 = new(BoundedRational.Quarter, CrPI);
    private static readonly UnifiedReal PIOver6 = new(BoundedRational.Sixth, CrPI);

    #endregion

    /// <summary>
    /// Kinds of properties we associated with constructive reals.
    /// </summary>
    /// <remarks>
    /// Revisit definitelyIndependent() below if anything is added here.
    /// We simplify/normalize where easily possible. E.g. we always represent exp(0) as 1,
    /// and ln(1/2) as -ln(2). That is reflected in constraints on the arguments below.
    /// For everything other than IS_ONE, we disallow arguments such that the property
    /// would describe a rational value.
    /// </remarks>
    internal enum PropertyKind : byte
    {
        // CR is 1.
        IsOne = 1,

        // CR is pi.
        IsPi = 2,

        // CR is sqrt(<rational>). Arg is > 0 and not 1.
        // The argument is minimal (doesn't contain square factors > 1)
        // if num and den < EXTRACT_SQUARE_MAX_OPT.
        // It is not an exact square.
        IsSqrt = 3,

        // CR is exp(<rational>). Arg is not 0.
        IsExp = 4,

        // CR is ln(<rational>). Arg is > 1.
        IsLn = 5,

        // CR is log10(<rational>). Arg is > 1, and not a power of 10.
        IsLog = 6,

        // CR is sin(pi*<rational>)
        // The rational argument is strictly between 0 and 0.5.
        // The argument is not equal to 1/6, 1/4, or 1/3.
        IsSinPi = 7,

        // CR is tan(pi*<rational>)
        // The rational argument is strictly between 0 and 0.5
        // The argument is not equal to zero or 1/6, 1/4, or 1/3.
        IsTanPi = 8,

        // CR is asin(<rational>)
        // The rational argument is strictly between 0 and 1, and not equal to 0.5.
        IsASin = 9,

        // CR is atan(<rational>)
        // The rational argument is > 0 and not equal to 1.
        IsATan = 10,

        // CR is irrational, but we otherwise know nothing about it.
        IsIrrational = 11
    }

    /// <summary>
    /// Properties associated with certain constructive reals.
    /// </summary>
    internal class CRProperty
    {
        public readonly PropertyKind Kind; // One of the above property kinds.
        public readonly BoundedRational Arg; // Reduced rational argument, if any.

        private static void CheckArg(PropertyKind kind, BoundedRational arg)
        {
            // Null check first
            if (arg.IsNull)
            {
                Check(kind == PropertyKind.IsOne || kind == PropertyKind.IsPi || kind == PropertyKind.IsIrrational);
                return;
            }

            // Now check the rest
            switch (kind)
            {
                case PropertyKind.IsOne:
                case PropertyKind.IsPi:
                case PropertyKind.IsIrrational:
                    Check(false); // arg.IsNull should be false
                    break;
                case PropertyKind.IsSqrt:
                    Check(arg.Sign > 0 && arg.CompareToOne() != 0);
                    break;
                case PropertyKind.IsExp:
                    Check(arg.Sign != 0);
                    break;
                case PropertyKind.IsLog:
                    Check(arg.CompareToOne() > 0 && IntLog10(arg) == 0 /* Not power of 10 */);
                    break;
                case PropertyKind.IsLn:
                    Check(arg.CompareToOne() > 0);
                    break;
                case PropertyKind.IsSinPi:
                case PropertyKind.IsTanPi:
                    Check(!CanTrigBeReduced(arg));
                    break;
                case PropertyKind.IsASin:
                    Check(arg.CompareTo(BoundedRational.MinusOne) > 0);
                    Check(arg.CompareTo(BoundedRational.One) < 0);
                    Check(arg.CompareTo(BoundedRational.MinusHalf) != 0);
                    Check(arg.CompareTo(BoundedRational.Half) != 0);
                    Check(arg.Sign != 0);
                    break;
                case PropertyKind.IsATan:
                    Check(arg.CompareTo(BoundedRational.MinusOne) != 0);
                    Check(arg.CompareTo(BoundedRational.One) != 0);
                    Check(arg.Sign != 0);
                    break;
                default:
                    throw new InvalidOperationException("Bad kind");
            }
        }

        public CRProperty(PropertyKind kind, BoundedRational arg)
        {
            CheckArg(kind, arg);
            Kind = kind;
            Arg = arg;
        }

        // The only instances of IS_ONE, IS_PI, and IS_IRRATIONAL properties.
        public static readonly CRProperty PropOne = new(PropertyKind.IsOne, BoundedRational.Null);
        public static readonly CRProperty PropPI = new(PropertyKind.IsPi, BoundedRational.Null);
        public static readonly CRProperty PropIrrational = new(PropertyKind.IsIrrational, BoundedRational.Null);

        public static readonly CRProperty PropSqrt2 = new(PropertyKind.IsSqrt, BoundedRational.Two);
        public static readonly CRProperty PropSqrt3 = new(PropertyKind.IsSqrt, BoundedRational.Three);
        public static readonly CRProperty PropExp = new(PropertyKind.IsExp, BoundedRational.One);
        public static readonly CRProperty PropLn10 = new(PropertyKind.IsLn, BoundedRational.Ten);

        // Does this property uniquely determine the number?
        internal bool DeterminesCr()
        {
            return Kind != PropertyKind.IsIrrational; // No other such properties yet.
        }

        public override int GetHashCode()
        {
            throw new InvalidOperationException("CRProperty hashCode used");
        }

        public override bool Equals(object? r)
        {
            if (r == this)
            {
                return true;
            }

            if (Kind == PropertyKind.IsOne /* optimization, there is only one such object */
                || r is not CRProperty prop)
            {
                return false;
            }

            return Kind == prop.Kind && Arg.Equals(prop.Arg);
        }

        /// <summary>
        /// Is the argument such that trig_func(pi*arg) can be simplified,
        /// or which should have been reduced to the (0, 1/2) interval.
        /// </summary>
        private static bool CanTrigBeReduced(BoundedRational arg)
        {
            return arg.Sign <= 0
                   || arg.CompareTo(BoundedRational.Half) >= 0
                   || arg.CompareTo(BoundedRational.Third) == 0
                   || arg.CompareTo(BoundedRational.Quarter) == 0
                   || arg.CompareTo(BoundedRational.Sixth) == 0;
        }
    }

    /// <summary>
    /// Reduce a SIN_PI or TAN_PI argument to the interval [-1/2, 1.5).
    /// Preserves null-ness. May produce null if BoundedRational arithmetic overflows.
    /// </summary>
    private static BoundedRational ReducedArg(BoundedRational arg)
    {
        if (arg.IsNull)
        {
            return BoundedRational.Null;
        }

        // Avoid real computations if we can. Only needed for performance.
        if (arg.CompareTo(BoundedRational.MinusHalf) >= 0 && arg.CompareTo(Br32) < 0)
        {
            return arg;
        }

        BoundedRational argPlusHalf = arg + BoundedRational.Half;
        if (argPlusHalf.IsNull)
        {
            return BoundedRational.Null;
        }

        BigInteger argPhFloor = argPlusHalf.Floor();
        BigInteger resultOffset = argPhFloor & ~BigInteger.One;
        return arg - new BoundedRational(resultOffset);
    }

    private static CRProperty MakeProperty(PropertyKind kind, BoundedRational arg)
    {
        // This enforces requirements on arg.
        if (kind == PropertyKind.IsOne || (kind == PropertyKind.IsSqrt && arg.Equals(BoundedRational.One))
                                       || (kind == PropertyKind.IsExp && arg.Sign == 0))
        {
            return CRProperty.PropOne;
        }

        if (kind == PropertyKind.IsPi)
        {
            return CRProperty.PropPI;
        }

        if (kind == PropertyKind.IsIrrational)
        {
            return CRProperty.PropIrrational;
        }

        return new CRProperty(kind, arg.Reduce());
    }

    /// <summary>
    /// Pair returned by trig normalization routines.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="sign"></param>
    private class SignedProperty(CRProperty p, bool sign)
    {
        public CRProperty Property { get; } = p;
        public bool Negative { get; } = sign;
    }

    /// <summary>
    /// Return a property corresponding to sin(pi*arg), normalizing arg to the correct range. Caller is
    /// responsible for ensuring that the argument is not one that leads to a rational result.
    /// The negate field of the result is set if the property corresponds to the negated argument,
    /// rather than the argument itself.
    /// Returns null if we can't normalize the argument.
    /// </summary>
    private static SignedProperty? MakeSinPiProperty(BoundedRational arg)
    {
        BoundedRational nArg = ReducedArg(arg);
        if (!nArg.IsNull)
        {
            bool neg = false;
            if (nArg.CompareTo(BoundedRational.Half) >= 0)
            {
                // sin(x) = sin(pi - x)
                nArg = BoundedRational.One - nArg;
            }

            if (nArg is { IsNull: false, Sign: < 0 })
            {
                nArg = -nArg;
                neg = true;
            }

            if (!nArg.IsNull)
            {
                return new SignedProperty(MakeProperty(PropertyKind.IsSinPi, nArg), neg);
            }
        }

        return null;
    }

    /// <summary>
    /// Return a property corresponding to tan(pi*arg), normalizing arg to the correct range. Caller is
    /// responsible for ensuring that the argument is not one that leads to a rational result.
    /// The negate field of the result is set if the property corresponds to the negated argument,
    /// rather than the argument itself.
    /// Returns null if we can't normalize the argument.
    /// </summary>
    private static SignedProperty? MakeTanPiProperty(BoundedRational arg)
    {
        BoundedRational nArg = ReducedArg(arg);
        if (!nArg.IsNull)
        {
            bool neg = false;
            if (nArg.CompareTo(BoundedRational.Half) >= 0)
            {
                // tan(x) = tan(x - pi)
                nArg = nArg - BoundedRational.One;
            }

            if (nArg is { IsNull: false, Sign: < 0 })
            {
                nArg = -nArg;
                neg = true;
            }

            if (!nArg.IsNull)
            {
                return new SignedProperty(MakeProperty(PropertyKind.IsTanPi, nArg), neg);
            }
        }

        return null;
    }

    // check methods which handle CRProperty as non-null

    private static bool IsOne(CRProperty p)
    {
        return p.Kind == PropertyKind.IsOne;
    }

    private static bool IsPi(CRProperty p)
    {
        return p.Kind == PropertyKind.IsPi;
    }

    private static bool IsUnknownIrrational(CRProperty p)
    {
        return p.Kind == PropertyKind.IsIrrational;
    }

    // Does p guarantee that the described constructive real is nonzero?
    private static bool IsNonzero(CRProperty? p)
    {
        if (p == null)
        {
            return false;
        }

        return p.Kind switch
        {
            PropertyKind.IsOne or PropertyKind.IsPi or PropertyKind.IsIrrational => true,
            PropertyKind.IsExp =>
                // It would be correct to always answer true. But we intentionally fail to provide the
                // guarantee for large negative arguments, since it would be expensive to actually
                // distinguish the value from zero, and answering true often results in such an attempt.
                p.Arg.CompareTo(new BoundedRational(-10000)) >= 0,
            PropertyKind.IsLn or PropertyKind.IsLog =>
                // arg > 1
                true,
            PropertyKind.IsSqrt =>
                // arg > 0
                true,
            PropertyKind.IsSinPi or PropertyKind.IsTanPi or PropertyKind.IsASin or PropertyKind.IsATan =>
                // arg != 0
                true,
            _ => throw new InvalidOperationException("isNonzero")
        };
    }

    /// <summary>
    /// Return the constructive real described by the property.
    /// </summary>
    /// <remarks>
    /// We could instead omit storing the CR
    /// value when we have a sufficiently descriptive property.  But that might hurt performance
    /// slightly, since we would sometimes lose the benefit of prior argument evaluations.
    /// </remarks>
    /// <param name="p"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static ConstructiveReal? CrFromProperty(CRProperty? p)
    {
        if (p == null)
        {
            return null;
        }

        return p.Kind switch
        {
            PropertyKind.IsIrrational => null,
            PropertyKind.IsOne => CrOne,
            PropertyKind.IsPi => CrPI,
            PropertyKind.IsExp => p.Arg.ToConstructiveReal().Exp(),
            PropertyKind.IsLn => p.Arg.ToConstructiveReal().Ln(),
            PropertyKind.IsLog => p.Arg.ToConstructiveReal().Ln().Divide(CrLn10),
            PropertyKind.IsSqrt => p.Arg.ToConstructiveReal().Sqrt(),
            PropertyKind.IsSinPi => p.Arg.ToConstructiveReal().Multiply(CrPI).Sin(),
            PropertyKind.IsTanPi => UnaryCRFunction.TanFunction.Execute(p.Arg.ToConstructiveReal().Multiply(CrPI)),
            PropertyKind.IsASin => p.Arg.ToConstructiveReal().Asin(),
            PropertyKind.IsATan => UnaryCRFunction.AtanFunction.Execute(p.Arg.ToConstructiveReal()),
            _ => throw new InvalidOperationException("crFromProperty")
        };
    }

    private static ConstructiveReal NonNullCrFromProperty(CRProperty p) =>
        CrFromProperty(p) ?? throw new ArgumentOutOfRangeException(nameof(p));

    /// <summary>
    /// Try to compute a property describing the constructive real.
    /// </summary>
    /// <remarks>
    /// Recognizes only a few specific constants.
    /// </remarks>
    /// <param name="cr"></param>
    /// <returns></returns>
    private static CRProperty? PropertyFor(ConstructiveReal cr) =>
        cr == CrOne ? CRProperty.PropOne :
        cr == CrPI ? CRProperty.PropPI :
        cr == CrSqrt2 ? CRProperty.PropSqrt2 :
        cr == CrSqrt3 ? CRProperty.PropSqrt3 :
        cr == CrExp ? CRProperty.PropExp :
        cr == CrLn10 ? CRProperty.PropLn10 : null;

    /// <summary>
    /// Check that if crProperty uniquely defines a constructive real, then crProperty
    /// and crFactor both describe approximately the same number.
    /// </summary>
    public bool PropertyCorrect(int precision)
    {
        ConstructiveReal? propertyCr = CrFromProperty(_crProperty);
        if (propertyCr == null)
        {
            return true;
        }

        int bound = MsbBound(_crProperty);
        if (bound != int.MinValue
            && propertyCr.Abs().CompareTo(ConstructiveReal.One.ShiftLeft(bound), precision) < 0)
        {
            // msbBound produced incorrect result.
            return false;
        }

        return _crFactor.CompareTo(propertyCr, precision) == 0;
    }

    // Return the argument if the property p has the given kind.
    private static BoundedRational GetArgForKind(CRProperty? p, PropertyKind kind)
    {
        if (p == null || p.Kind != kind)
        {
            return BoundedRational.Null;
        }

        return p.Arg;
    }

    // The following return the rational argument if the property describes an application of
    // the matching function, null if not.

    private static BoundedRational GetSqrtArg(CRProperty? p)
    {
        return GetArgForKind(p, PropertyKind.IsSqrt);
    }

    private static BoundedRational GetExpArg(CRProperty? p)
    {
        return GetArgForKind(p, PropertyKind.IsExp);
    }

    private static BoundedRational GetLnArg(CRProperty? p)
    {
        return GetArgForKind(p, PropertyKind.IsLn);
    }

    private static BoundedRational GetLogArg(CRProperty? p)
    {
        return GetArgForKind(p, PropertyKind.IsLog);
    }

    private static BoundedRational GetSinPiArg(CRProperty? p)
    {
        return GetArgForKind(p, PropertyKind.IsSinPi);
    }

    private static BoundedRational GetTanPiArg(CRProperty? p)
    {
        return GetArgForKind(p, PropertyKind.IsTanPi);
    }

    private static BoundedRational GetASinArg(CRProperty? p)
    {
        return GetArgForKind(p, PropertyKind.IsASin);
    }

    private static BoundedRational GetATanArg(CRProperty? p)
    {
        return GetArgForKind(p, PropertyKind.IsATan);
    }

    // The (in abs value) integral exponent for which we attempt to use a recursive
    // algorithm for evaluating pow(). The recursive algorithm works independent of the sign of the
    // base, and can produce rational results. But it can become slow for very large exponents.
    private static readonly BigInteger RecursivePowLimit = new(1000);

    // The corresponding limit when we're using rational arithmetic. This should fail fast
    // anyway, but we avoid ridiculously deep recursion.
    private static readonly BigInteger HardRecursivePowLimit = BigInteger.One << 1000;

    // In some cases we cowardly refuse to compute answers longer than BIT_LIMIT, normally because
    // doing so is likely to cause us to run out of space in unpleasant ways.
    private static readonly int BitLimit = 2_000_000;
    private static readonly UnifiedReal BitLimitAsUReal = new(BitLimit);

    // Number of extra bits used in ToStringTruncated evaluation to prefer truncation to
    // rounding. Must be <= 30.
    private static readonly int ExtraPrecision = 10;

    private readonly BoundedRational _ratFactor;
    private readonly ConstructiveReal _crFactor;
    private readonly CRProperty? _crProperty;

    private static void Check(bool b)
    {
        Debug.Assert(b);
    }

    private UnifiedReal(BoundedRational rat, ConstructiveReal cr, CRProperty? p)
    {
        Check(rat != BoundedRational.Null);
        _crFactor = cr;
        _ratFactor = rat;
        _crProperty = p;
    }

    // Shorthand constructor; computes non-null property only for a few special cases.
    private UnifiedReal(BoundedRational rat, ConstructiveReal cr)
        : this(rat, cr, PropertyFor(cr))
    {
    }

    /// <summary>
    /// Construct a UnifiedReal from a ConstructiveReal.
    /// </summary>
    /// <param name="cr"></param>
    public UnifiedReal(ConstructiveReal cr)
        : this(BoundedRational.One, cr)
    {
    }

    private UnifiedReal(ConstructiveReal cr, CRProperty? p)
        : this(BoundedRational.One, cr, p)
    {
    }

    private UnifiedReal(BoundedRational rat, CRProperty p)
        : this(rat, NonNullCrFromProperty(p), p)
    {
        Check(p.DeterminesCr());
    }

    private UnifiedReal(CRProperty p)
        : this(BoundedRational.One, p)
    {
    }

    /// <summary>
    /// Construct a UnifiedReal from a BoundedRational.
    /// </summary>
    /// <param name="rat"></param>
    public UnifiedReal(BoundedRational rat)
        : this(rat, ConstructiveReal.One, CRProperty.PropOne)
    {
    }

    /// <summary>
    /// Construct a UnifiedReal from a BigInteger.
    /// </summary>
    /// <param name="n"></param>
    public UnifiedReal(BigInteger n)
        : this(new BoundedRational(n))
    {
    }

    /// <summary>
    /// Construct a UnifiedReal from a long.
    /// </summary>
    /// <param name="n"></param>
    public UnifiedReal(long n)
        : this(new BoundedRational(n))
    {
    }

    /// <summary>
    /// Construct a UnifiedReal from a double.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static UnifiedReal FromDouble(double x) =>
        x is 0.0 or 1.0
            ? FromLong((long)x)
            : new UnifiedReal(BoundedRational.FromDouble(x));

    /// <summary>
    /// Construct a UnifiedReal from a long.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static UnifiedReal FromLong(long n) =>
        n switch
        {
            0 => Zero,
            1 => One,
            _ => new UnifiedReal(n)
        };

    /// <summary>
    /// Return a string describing r*pi radians. If degrees is true describe the
    /// given number of radians in units of degrees.
    /// </summary>
    private static string SymbolicPiMultiple(BoundedRational r, bool degrees, bool unicodeFraction)
    {
        if (degrees)
        {
            BoundedRational rInDegrees = r * Br180;
            if (!rInDegrees.IsNull)
            {
                return rInDegrees.ToDisplayString(unicodeFraction /* mixed fractions */);
            }

            // Extremely unlikely, and the result isn't very useful. The result will be huge,
            // may not be fully reduced, and may be ugly, but it is correct.
            var numDen = r.NumDen;
            return numDen.Item1 * Big180 + "/" + numDen.Item2;
        }
        else
        {
            var numDen = r.NumDen;
            if (numDen.Item2 == BigInteger.One)
            {
                return numDen.Item1 + PIString;
            }

            if (unicodeFraction && numDen.Item1 != BigInteger.One)
            {
                return r.ToDisplayString(unicodeFraction /* mixed fractions */) + PIString;
            }

            return ((numDen.Item1 == BigInteger.One ? "" : numDen.Item1.ToString()) + PIString +
                    "/" + numDen.Item2);
        }
    }

    /// <summary>
    /// If the property indicates the constructive real has a simple symbolic representation, return
    /// it. The name is intended to be appended to the rational multiplier, so the name of one is the
    /// empty string. If unicodeFraction is true, use subscripts and superscripts to render the
    /// embedded rational.
    /// </summary>
    private static string? CrSymbolic(CRProperty? p, bool degrees, bool unicodeFraction)
    {
        // TODO (H. Boehm): This currently ignores translation issues. Revisit if/when Calculator exposes this to the user again.
        if (p == null || IsUnknownIrrational(p))
        {
            // Not enough information for a symbolic representation.
            return null;
        }

        if (IsOne(p))
        {
            return "";
        }

        if (IsPi(p))
        {
            return PIString;
        }

        BoundedRational expArg = GetExpArg(p);
        if (!expArg.IsNull)
        {
            return expArg.Equals(BoundedRational.One)
                ? "e"
                : $"exp({expArg.ToDisplayString(unicodeFraction /* mixed */)})";
        }

        BoundedRational sqrtArg = GetSqrtArg(p);
        if (!sqrtArg.IsNull)
        {
            BigInteger? intSqrtArg = sqrtArg.ToBigInteger();
            return intSqrtArg != null
                ? $"{SqrtString}{intSqrtArg}"
                : $"{SqrtString}({sqrtArg.ToDisplayString(unicodeFraction)})";
        }

        BoundedRational lnArg = GetLnArg(p);
        if (!lnArg.IsNull)
        {
            return $"ln({lnArg.ToDisplayString(unicodeFraction)})";
        }

        BoundedRational logArg = GetLogArg(p);
        if (!logArg.IsNull)
        {
            return $"log({logArg.ToDisplayString(unicodeFraction)})";
        }

        BoundedRational sinPiArg = GetSinPiArg(p);
        if (!sinPiArg.IsNull)
        {
            return $"sin({SymbolicPiMultiple(sinPiArg, degrees, unicodeFraction)})";
        }

        BoundedRational tanPiArg = GetTanPiArg(p);
        if (!tanPiArg.IsNull)
        {
            return $"tan({SymbolicPiMultiple(tanPiArg, degrees, unicodeFraction)})";
        }

        BoundedRational aSinArg = GetASinArg(p);
        if (!aSinArg.IsNull)
        {
            // sin superscript -1
            return
                $"sin{InverseString}({aSinArg.ToDisplayString(unicodeFraction)}){(degrees ? DegreeConversion : "")}";
        }

        BoundedRational aTanArg = GetATanArg(p);
        if (!aTanArg.IsNull)
        {
            return
                $"tan{InverseString}({aTanArg.ToDisplayString(unicodeFraction)}){(degrees ? DegreeConversion : "")}";
        }

        return null;
    }

    private const string PIString = "\u03C0"; // GREEK SMALL LETTER PI
    private const string SqrtString = "\u221A";
    private const string MultString = "\u00D7";
    private const string InverseString = "\u207B\u00B9"; // -1 superscript
    private const string DegreeConversion = MultString + "180/" + PIString;

    /// <summary>
    /// Is this number known to be algebraic?
    /// </summary>
    public bool DefinitelyAlgebraic()
    {
        return DefinitelyAlgebraic(_crProperty) || _ratFactor.Sign == 0;
    }

    /// <summary>
    /// Is cr known to be algebraic (as opposed to transcendental)? Currently only produces meaningful
    /// results for the above known special constructive reals.
    /// </summary>
    private static bool DefinitelyAlgebraic(CRProperty? p)
    {
        return p is { Kind: PropertyKind.IsOne or PropertyKind.IsSqrt or PropertyKind.IsSinPi or PropertyKind.IsTanPi };
    }

    /// <summary>
    /// Is this number known to be rational?
    /// </summary>
    public bool DefinitelyRational() => _crProperty != null && (IsOne(_crProperty) || _ratFactor.Sign == 0);

    /// <summary>
    /// Is this number known to be irrational?
    /// </summary>
    public bool DefinitelyIrrational()
    {
        return _crProperty != null && _crProperty.Kind != PropertyKind.IsOne;
        // Clearly correct for IS_IRRATIONAL.
        // The other kinds carefully exclude arguments describing a rational value.
    }

    /// <summary>
    /// Is this number known to be transcendental?
    /// </summary>
    public bool DefinitelyTranscendental()
    {
        if (DefinitelyRational() || _crProperty == null)
        {
            return false;
        }
        // If crProperty.Kind == PropertyKind.IsOne, it can't go here.

        switch (_crProperty.Kind)
        {
            case PropertyKind.IsPi:
                return true;
            case PropertyKind.IsSqrt:
                return false;
            case PropertyKind.IsLn:
                // arg > 1
                // Follows from Lindemann-Weierstrass theorem. If ln(r) = p, where r is rational, and
                // p is algebraic, then r = e^p. But if p is nonzero algebraic, then e^p is transcendental.
                return true;
            case PropertyKind.IsLog:
                // If this is rational, then n ln(arg) = m ln(10), n and m integers.
                // TODO: Can we do better?
                return false;
            case PropertyKind.IsExp:
                // arg != 0
                // Simple application of Lindemann-Weierstrass theorem.
                return true;
            case PropertyKind.IsSinPi:
            case PropertyKind.IsTanPi:
                // Always algebraic for rational multiples of pi.
                return false;
            case PropertyKind.IsASin:
            case PropertyKind.IsATan:
                // If asin(r) = theta, r rational, theta algebraic, then r = sin(theta). It follows from
                // Lindemann-Weierstrass that this can happen only if theta is zero, i.e. if r is zero.
                // We don't use this representation for asin(0). The atan argument is similar.
                return true;
            case PropertyKind.IsIrrational:
                // Not enough information to tell.
                return false;
            case PropertyKind.IsOne:
            default:
                throw new InvalidOperationException("Unexpected PropertyKind"); // Can't get here.
        }
    }

    /// <summary>
    /// Is the supplied IS_SQRT property argument known to be minimal?
    /// </summary>
    private static bool IrreducibleSqrt(BoundedRational sqrtArg)
    {
        var nd = sqrtArg.NumDen;
        // Check absolute values without allocation.
        return nd.Item1.GetBitLength() <= 30
               && Math.Abs((int)nd.numerator) <= BoundedRational.ExtractSquareMaxOpt
               && nd.Item2.GetBitLength() <= 30
               && Math.Abs((int)nd.denominator) <= BoundedRational.ExtractSquareMaxOpt;
    }

    /// <summary>
    /// Return a rational r != 0, such that x = y^r, or null if we didn't find one. Effectively this
    /// tests whether x and y have a common integral power and returns the two exponents as a rational.
    /// x and y are presumed to be positive. Note that if x = y = 1, we return 1, but any rational
    /// would do. We do not try very hard if any of the numbers involved are large.
    /// </summary>
    private static BoundedRational CommonPower(BigInteger x, BigInteger y)
    {
        int compareResult = x.CompareTo(y);
        if (compareResult == 0)
        {
            return BoundedRational.One;
        }

        if (compareResult < 0)
        {
            // BoundedRational already propagates null as desired.
            BoundedRational inverse = CommonPower(y, x);
            return !inverse.IsNull ? BoundedRational.Inverse(inverse) : BoundedRational.Null;
        }

        if (x.CompareTo(BigInteger.One) == 0 || y.CompareTo(BigInteger.One) == 0)
        {
            return BoundedRational.Null;
        }

        if (x.GetBitLength() > CommonPowerLengthLimit)
        {
            // punt
            return BoundedRational.Null;
        }

        // We use a modified version of the Euclidean GCD algorithm, repeatedly dividing the larger
        // number by the smaller. If x = y^r, then (x/y) = y^(r-1).
        BigInteger quotient = BigInteger.DivRem(x, y, out var remainder);
        if (remainder != 0)
        {
            // If they're not divisible, there must be two primes, such that x is divisible by a larger
            // power of one than y and vice versa. That makes it impossible that x^n = y^m, m and n
            // integers. thus we know r doesn't exist.
            return BoundedRational.Null;
        }

        BoundedRational recursiveResult = CommonPower(quotient, y);
        return !recursiveResult.IsNull ? recursiveResult + BoundedRational.One : BoundedRational.Null;
    }

    /// <summary>
    /// Do a and b have a common power? Considers negative exponents. Both a and b must be positive.
    /// </summary>
    public static bool CommonPower(BoundedRational a, BoundedRational b)
    {
        var (numerator, denominator) = a.NumDen;
        var ndb = b.NumDen;
        // First consider the case in which one numerator and/or denominator pair consists
        // entirely of ones. This is special because commonPower() is not uniquely determined.
        if (denominator.CompareTo(BigInteger.One) == 0)
        {
            if (ndb.denominator.CompareTo(BigInteger.One) == 0)
            {
                return !CommonPower(numerator, ndb.numerator).IsNull;
            }

            if (ndb.numerator.CompareTo(BigInteger.One) == 0)
            {
                return !CommonPower(numerator, ndb.denominator).IsNull;
            }
        }
        else if (numerator.CompareTo(BigInteger.One) == 0)
        {
            if (ndb.numerator.CompareTo(BigInteger.One) == 0)
            {
                return !CommonPower(denominator, ndb.denominator).IsNull;
            }

            if (ndb.denominator.CompareTo(BigInteger.One) == 0)
            {
                return !CommonPower(denominator, ndb.numerator).IsNull;
            }
        }

        // In the general case, two commonPower computations must produce the same result.
        BoundedRational commonPowerNaNb = CommonPower(numerator, ndb.numerator);
        BoundedRational commonPowerNaDb = CommonPower(numerator, ndb.denominator);
        if (!commonPowerNaNb.IsNull && commonPowerNaNb.Equals(CommonPower(denominator, ndb.denominator)))
        {
            // They have a common power, and both exponents have the same sign.
            return true;
        }

        if (!commonPowerNaDb.IsNull && commonPowerNaDb.Equals(CommonPower(denominator, ndb.numerator)))
        {
            // They have a common power, and exponents have different sign.
            return true;
        }

        return false;
    }

    private static readonly int CommonPowerLengthLimit = 200;

    /// <summary>
    /// Do we know that this.crFactor is an irrational nonzero multiple of u.crFactor? If this returns
    /// true, then a comparison of the two UnifiedReals cannot diverge, though we don't know of a good
    /// runtime bound. Note that if both values are really tiny, it still may be completely impractical
    /// to compare them.
    /// </summary>
    public bool DefinitelyIndependent(UnifiedReal u)
    {
        // We always return false if either crFactor might be zero.
        CRProperty? p1 = _crProperty;
        CRProperty? p2 = u._crProperty;
        if (p1 == null || p2 == null || p1.Equals(p2))
        {
            return false;
        }

        // Halve the number of cases. ONE < PI < SQRT < EXP < LN.
        if ((int)p1.Kind > (int)p2.Kind)
        {
            return u.DefinitelyIndependent(this);
        }

        switch (p1.Kind)
        {
            case PropertyKind.IsOne:
                return u.DefinitelyIrrational();
            case PropertyKind.IsPi:
                // It appears to be unknown whether pi is a rational multiple of an exponential or log.
                // If we were brave, we could say true, and hope for an infinite loop, which would
                // probably prove an interesting theorem. But we are not ...
                // IS_ONE case is already handled, since p1 <= p2.
                return p2.Kind == PropertyKind.IsSqrt;
            case PropertyKind.IsSqrt:
                if (u.DefinitelyTranscendental())
                {
                    return true;
                }

                if (p2.Kind == PropertyKind.IsSqrt)
                {
                    // The argument is not necessarily minimal.
                    return IrreducibleSqrt(p1.Arg) && IrreducibleSqrt(p2.Arg) && !p1.Equals(p2);
                }

                return false;
            case PropertyKind.IsExp:
                if (p2.Kind == PropertyKind.IsExp)
                {
                    // Lindemann-Weierstrass theorem gives us algebraic independence.
                    return !p1.Arg.Equals(p2.Arg);
                }

                if (p2.Kind == PropertyKind.IsLn)
                {
                    // If e^a = cln(b), then e^e^a = b^c. The r.h.s is an algebraic multiple of e^0.
                    // By Lindemann-Weierstrass, this can only happen if e^a = 0, which is impossible.
                    return true;
                }

                return u.DefinitelyAlgebraic();
            case PropertyKind.IsLn:
                if (p2.Kind == PropertyKind.IsIrrational)
                {
                    return false; // Not enough information.
                }

                if (p2.Kind == PropertyKind.IsLn)
                {
                    // If ln(a) = cln(b), then a = b^c, a, b, and c rational, or equivalently a^c1 = b^c2,
                    // with c1 and c2 integers. C must be nonzero, since a > 1.  A necessary condition for
                    // this is that the numerator and denominator separately have to have a common integral
                    // power.
                    return !CommonPower(p1.Arg, p2.Arg);
                }

                // Assume ln(r) = a is algebraic. Then e^a is rational. By Lindemann-Weierstrass, this
                // implies a = 0 and r = 1. We know that the argument is not one, so any algebraic
                // number must be linearly independent over the rationals.
                return u.DefinitelyAlgebraic();
            // TODO: Can we do better for IS_LOG?
            case PropertyKind.IsLog:
                // In the irrational case, with u rational, we would have checked in the other order.
                if (p2.Kind == PropertyKind.IsLog)
                {
                    // We're asking if ln(a)/ln(10) = r ln(b)/ln(10), which is true iff ln(a) = r ln(b).
                    // Use the same algorithm as for IS_LN.
                    return !CommonPower(p1.Arg, p2.Arg);
                }

                return false;
            case PropertyKind.IsSinPi:
            case PropertyKind.IsTanPi:
                // Always algebraic. We already handled the u rational case above.
                return u.DefinitelyTranscendental();
            case PropertyKind.IsASin:
                // As we argued above, this is transcendental.
                return u.DefinitelyAlgebraic();
            case PropertyKind.IsATan:
                // The case of u rational is handled above. Can we do better?
                return false;
            case PropertyKind.IsIrrational:
                return false;
            default:
                throw new InvalidOperationException("Unexpected PropertyKind");
        }
    }

    /// <summary>
    /// Convert to String reflecting raw representation. Debug or log messages only, not pretty.
    /// </summary>
    public override string ToString() => $"{_ratFactor}*{_crFactor}";

    /// <summary>
    /// Convert to readable String. Intended for user output. Produces exact expression when possible.
    /// If degrees is true, then any trig functions in the output will refer to the degree versions.
    /// Otherwise, the radian versions are used.
    /// </summary>
    public string ToDisplayString(bool degrees, bool unicodeFraction, bool mixed)
    {
        if (_crProperty != null && (IsOne(_crProperty) || _ratFactor.Sign == 0))
        {
            return _ratFactor.ToDisplayString(unicodeFraction, mixed);
        }

        string? symbolic = CrSymbolic(_crProperty, degrees, unicodeFraction);
        if (symbolic != null)
        {
            BigInteger? bi = _ratFactor.ToBigInteger();
            if (bi != null)
            {
                if (bi == BigInteger.One)
                {
                    return symbolic;
                }

                if (bi == BigMinusOne)
                {
                    return $"-{symbolic}";
                }

                return bi + symbolic;
            }

            BigInteger? biInverse = BoundedRational.Inverse(_ratFactor).ToBigInteger();
            if (biInverse != null)
            {
                // Use spaces to reduce ambiguity with square roots.
                return $"{(biInverse < 0 ? "-" : "")}{symbolic} / {BigInteger.Abs(biInverse.Value)}";
            }

            if (unicodeFraction)
            {
                return $"{_ratFactor.ToDisplayString(unicodeFraction /* mixed */)}{symbolic}";
            }

            return $"({_ratFactor.ToDisplayString()}){symbolic}";
        }

        if (_ratFactor.Equals(BoundedRational.One))
        {
            return _crFactor.ToString();
        }

        return ToConstructiveReal().ToString();
    }

    /// <summary>
    /// Convert to a readable string using radian version of trig functions.
    /// Use improper fractions, and no sub-and super-scripts.
    /// </summary>
    public string ToDisplayString() => ToDisplayString(false, false, false);

    /// <summary>
    /// Will ToDisplayString() produce an exact representation?
    /// </summary>
    public bool ExactlyDisplayable() => _crProperty != null && _crProperty.DeterminesCr();

    /// <summary>
    /// Return summary statistics for the expression returned by ToDisplayString().
    /// Assumes ExactlyDisplayable() is true.
    /// </summary>
    internal ExprStats GetStats(bool degrees)
    {
        ExprStats result = new ExprStats();
        BoundedRational normalized = _ratFactor.Reduce();
        BigInteger? bi = normalized.ToBigInteger();
        result.TotalConstBitLength = (int?)bi?.GetBitLength() ?? normalized.BitLength;
        result.NOps = (bi != null ? 0 : 1);
        if (_crProperty == null) return result;
        switch (_crProperty.Kind)
        {
            case PropertyKind.IsOne:
                break;
            case PropertyKind.IsPi:
                ++result.NOps;
                ++result.NCommonOps;
                break;
            case PropertyKind.IsExp:
                if (_crProperty.Arg.Equals(BoundedRational.One))
                {
                    // Displayed as "e".
                    ++result.NOps;
                    ++result.NCommonOps;
                    break;
                } // else fall through:

                goto case PropertyKind.IsLn;
            case PropertyKind.IsLn:
            case PropertyKind.IsLog:
            case PropertyKind.IsSinPi:
            case PropertyKind.IsTanPi:
            case PropertyKind.IsASin:
            case PropertyKind.IsATan:
                result.TranscendentalFns++;
                // And fall through:
                goto case PropertyKind.IsSqrt;
            case PropertyKind.IsSqrt:
                BoundedRational arg = _crProperty.Arg;
                if (degrees && (_crProperty.Kind == PropertyKind.IsSinPi || _crProperty.Kind == PropertyKind.IsTanPi))
                {
                    // Arg will be converted to degrees on output.
                    arg = (arg * Br180).Reduce();
                }

                BigInteger? argAsBigInt = arg.ToBigInteger();
                int argBitLength = (int?)argAsBigInt?.GetBitLength() ?? arg.BitLength;
                result.TotalConstBitLength += argBitLength;
                result.InterestingConstBitLength = argBitLength;
                result.NOps += (argAsBigInt != null ? 1 : 2); // 1 for main function, maybe 1 for quotient.
                if (degrees)
                {
                    if (_crProperty.Kind == PropertyKind.IsASin || _crProperty.Kind == PropertyKind.IsATan)
                    {
                        // Need to add ugly conversion on output.
                        result.TotalConstBitLength += (int)Big180.GetBitLength();
                        result.NOps += 3; // Multiplication, division, pi.
                    }
                }
                else
                {
                    // The result expression needs a pi behind the argument.
                    result.NOps += 1;
                }

                break;
            default:
                throw new InvalidOperationException("Unexpected kind " + _crProperty.Kind);
        }

        return result;
    }

    /// <summary>
    /// Returns a truncated representation of the result.
    /// If <see cref="ExactlyTruncatable"/>, we round correctly towards zero. Otherwise, the resulting digit
    /// string may occasionally be rounded up instead.
    /// Always includes a decimal point in the result.
    /// The result includes n digits to the right of the decimal point.
    /// </summary>
    /// <param name="n">result precision, >= 0</param>
    public string ToStringTruncated(int n)
    {
        if (_crProperty != null && (IsOne(_crProperty) || _ratFactor.Sign == 0))
        {
            return _ratFactor.ToStringTruncated(n);
        }

        ConstructiveReal scaled = ConstructiveReal.FromBigInteger(BigInteger.Pow(10, n))
            .Multiply(ToConstructiveReal());
        bool negative = false;
        BigInteger intScaled;
        if (ExactlyTruncatable())
        {
            intScaled = scaled.GetApproximation(0);
            if (intScaled.Sign < 0)
            {
                negative = true;
                intScaled = -intScaled;
            }

            if (ConstructiveReal.FromBigInteger(intScaled).CompareTo(scaled.Abs()) > 0)
            {
                intScaled -= BigInteger.One;
            }

            Check(ConstructiveReal.FromBigInteger(intScaled).CompareTo(scaled.Abs()) < 0);
        }
        else
        {
            // Approximate case.  Exact comparisons are impossible.
            intScaled = scaled.GetApproximation(-ExtraPrecision);
            if (intScaled.Sign < 0)
            {
                negative = true;
                intScaled = -intScaled;
            }

            intScaled >>= ExtraPrecision;
        }

        string digits = intScaled.ToString();
        int len = digits.Length;
        if (len < n + 1)
        {
            digits = new string('0', n + 1 - len) + digits;
            len = n + 1;
        }

        return $"{(negative ? "-" : "")}{digits.Substring(0, len - n)}.{digits.Substring(len - n)}";
    }

    /// <summary>
    /// Can we compute correctly truncated approximations of this number?
    /// </summary>
    public bool ExactlyTruncatable()
    {
        // If the value is known rational, we can do exact comparisons.
        // If the value is known irrational, then we can safely compare to rational approximations;
        // equality is impossible; hence the comparison must converge.
        // The only problem cases are the ones in which we don't know.
        return _crProperty != null && (IsOne(_crProperty) || _ratFactor.Sign == 0 || DefinitelyIrrational());
    }

    /// <summary>
    /// Return a double approximation. Rational arguments are currently rounded to nearest, with ties
    /// away from zero. TODO: Improve rounding.
    /// </summary>
    public double ToDouble() =>
        _crProperty != null && IsOne(_crProperty)
            ? _ratFactor.ToDouble() // Hopefully correctly rounded
            : ToConstructiveReal().DoubleValue(); // Approximately correctly rounded

    /// <summary>
    /// Convert to a ConstructiveReal representation
    /// </summary>
    public ConstructiveReal ToConstructiveReal() =>
        _ratFactor.CompareToOne() == 0 ? _crFactor : _ratFactor.ToConstructiveReal().Multiply(_crFactor);

    private bool SameCrFactor(UnifiedReal u)
    {
        return ReferenceEquals(_crFactor, u._crFactor)
               || (_crProperty != null && _crProperty.DeterminesCr() && _crProperty.Equals(u._crProperty));
    }

    /// <summary>
    /// Do both numbers have properties of the same kind describing either a constant or
    /// a strictly monotonic function?
    /// </summary>
    /// <remarks>
    /// All of our kinds other than IS_IRRATIONAL currently qualify.
    /// </remarks>
    private bool SameMonotonicCrKind(UnifiedReal u) =>
        _crProperty != null && u._crProperty != null &&
        _crProperty.Kind == u._crProperty.Kind &&
        _crProperty.DeterminesCr();

    /// <summary>
    /// Are this and r exactly comparable?
    /// </summary>
    public bool IsComparable(UnifiedReal u)
    {
        // We check for ONE only to speed up the common case.
        // The use of a tolerance here means we can spuriously return false, not true.
        return (SameCrFactor(u) && IsNonzero(_crProperty))
               || _ratFactor.Sign == 0 && u._ratFactor.Sign == 0
               || (DefinitelyIndependent(u)
                   // One of the operands also needs to be non-tiny for the comparison to be practical.
                   && (LeadingBinaryZeroes() < -ZeroComparisonTolerance
                       || u.LeadingBinaryZeroes() < -ZeroComparisonTolerance
                       || ToConstructiveReal().Sign(DefaultInitialTolerance) != 0 // Try cheaper test first.
                       || u.ToConstructiveReal().Sign(DefaultInitialTolerance) != 0
                       || ToConstructiveReal().Sign(ZeroComparisonTolerance) != 0
                       || u.ToConstructiveReal().Sign(ZeroComparisonTolerance) != 0))
               || (SameMonotonicCrKind(u) && // _crProperty != null && u._crProperty != null
                   (_ratFactor.Equals(u._ratFactor) || _crProperty!.Kind == PropertyKind.IsSqrt))
               || ToConstructiveReal().CompareTo(u.ToConstructiveReal(),
                   DefaultRelativeTolerance, DefaultComparisonTolerance) != 0;
    }

    /// <summary>
    /// Return +1 if this is greater than r, -1 if this is less than r, or 0 of the two are known to be
    /// equal. May diverge if the two are equal and !isComparable(r).
    /// </summary>
    public int CompareTo(UnifiedReal u)
    {
        if (DefinitelyZero() && u.DefinitelyZero())
        {
            return 0;
        }

        if (SameCrFactor(u))
        {
            int sign = _crFactor.Sign(); // Can diverge if crFactor == 0.
            return sign * _ratFactor.CompareTo(u._ratFactor);
        }

        if (SameMonotonicCrKind(u))
        {
            Debug.Assert(_crProperty != null && u._crProperty != null);
            if (_ratFactor.Equals(u._ratFactor))
            {
                // kind cannot be IS_PI or IS_ONE, since sameCrFactor() would have been true.
                // sameMonotonicCrKind() precludes IS_IRRATIONAL.
                // All other kinds represent monotonically increasing functions over the range we allow.
                // Just compare the arguments.
                return _ratFactor.Sign * _crProperty.Arg.CompareTo(u._crProperty.Arg);
            }

            if (_crProperty.Kind == PropertyKind.IsSqrt)
            {
                // Compare the squares. We promise to compare these accurately, so we force
                // the multiplications to succeed by letting the result exceed BoundedRational
                // size bounds.
                int sign = _ratFactor.Sign;
                int uSign = u._ratFactor.Sign;
                if (sign < uSign)
                {
                    return -1;
                }

                if (sign > uSign)
                {
                    return 1;
                }

                BoundedRational sqrtArg = GetSqrtArg(_crProperty);
                BoundedRational squared = BoundedRational.RawMultiply(
                    BoundedRational.RawMultiply(_ratFactor, _ratFactor), sqrtArg);
                BoundedRational uSqrtArg = GetSqrtArg(u._crProperty);
                BoundedRational uSquared = BoundedRational.RawMultiply(
                    BoundedRational.RawMultiply(u._ratFactor, u._ratFactor), uSqrtArg);
                // Both squared and uSquared are non-null.
                return sign * squared.CompareTo(uSquared);
            }
        }

        return ToConstructiveReal().CompareTo(u.ToConstructiveReal()); // Can also diverge.
    }

    /// <summary>
    /// Return +1 if this is greater than r, -1 if this is less than r, 0 if the two are equal, or
    /// possibly 0 if the two are within 2^a of each other, and not comparable.
    /// </summary>
    public int CompareTo(UnifiedReal u, int a)
    {
        if (IsComparable(u))
        {
            return CompareTo(u);
        }

        // See if we can resolve comparison with lower precision first.
        for (int precision = DefaultInitialTolerance; precision * 2 > a; precision *= 2)
        {
            int result = ToConstructiveReal().CompareTo(u.ToConstructiveReal(), precision);
            if (result != 0) return result;
        }

        return ToConstructiveReal().CompareTo(u.ToConstructiveReal(), a);
    }

    /// <summary>
    /// Return CompareTo(Zero, a).
    /// </summary>
    public int ApproxSign(int a)
    {
        return CompareTo(Zero, a);
    }

    /// <summary>
    /// Return CompareTo(Zero). May diverge for Zero argument if !isComparable(Zero).
    /// </summary>
    public int DefinitelySign() => CompareTo(Zero);

    /// <summary>
    /// Equality comparison. May erroneously return true if values differ by less than 2^a, and
    /// !isComparable(u).
    /// </summary>
    public bool ApproxEquals(UnifiedReal u, int a)
    {
        if (IsComparable(u))
        {
            if (DefinitelyIndependent(u) && (_ratFactor.Sign != 0 || u._ratFactor.Sign != 0))
            {
                // No need to actually evaluate, though we don't know which is larger.
                return false;
            }

            return CompareTo(u) == 0;
        }

        return ToConstructiveReal().CompareTo(u.ToConstructiveReal(), a) == 0;
    }

    /// <summary>
    /// Returns true if values are definitely known to be equal, false in all other cases. This does
    /// not satisfy the contract for Object.equals().
    /// </summary>
    public bool DefinitelyEquals(UnifiedReal u) => IsComparable(u) && CompareTo(u) == 0;

    /// <summary>
    /// UnifiedReals don't have equality or hash codes.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override int GetHashCode()
    {
        throw new InvalidOperationException("UnifiedReals don't have equality or hash codes");
    }

    /// <summary>
    /// UnifiedReals don't have equality or hash codes.
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public override bool Equals(object? r)
    {
        if (r is not UnifiedReal)
        {
            return false;
        }

        // This is almost certainly a programming error. Don't even try.
        throw new InvalidOperationException("Can't compare UnifiedReals for exact equality");
    }

    /// <summary>
    /// Returns true if values are definitely known not to be equal based on internal symbolic
    /// information, false in all other cases. Performs no approximate evaluation.
    /// </summary>
    public bool DefinitelyNotEquals(UnifiedReal u)
    {
        if (_ratFactor.Sign == 0)
        {
            return IsNonzero(u._crProperty) && u._ratFactor.Sign != 0;
        }

        if (u._ratFactor.Sign == 0)
        {
            return IsNonzero(_crProperty) && _ratFactor.Sign != 0;
        }

        if (_crProperty == null || u._crProperty == null)
        {
            return false;
        }

        if (DefinitelyIndependent(u))
        {
            return _ratFactor.Sign != 0 || u._ratFactor.Sign != 0;
        }

        if (SameCrFactor(u) && IsNonzero(_crProperty))
        {
            return !_ratFactor.Equals(u._ratFactor);
        }

        return false;
    }

    // And some slightly faster convenience functions for special cases:

    /// <summary>
    /// Can this number be determined to be definitely zero without performing approximate
    /// evaluation?
    /// </summary>
    /// <returns></returns>
    public bool DefinitelyZero()
    {
        // If crFactor were known to be zero, we would have used a different representation.
        return _ratFactor.Sign == 0;
    }

    /// <summary>
    /// Can this number be determined to be definitely nonzero without performing approximate evaluation?
    /// </summary>
    public bool DefinitelyNonzero()
    {
        return IsNonzero(_crProperty) && _ratFactor.Sign != 0;
    }

    /// <summary>
    /// Can this number be determined to be definitely one without performing approximate evaluation?
    /// </summary>
    /// <returns></returns>
    public bool DefinitelyOne() => _crProperty != null && IsOne(_crProperty) && _ratFactor.Equals(BoundedRational.One);

    /// <summary>
    /// Return equivalent BoundedRational, if known to exist, null otherwise
    /// </summary>
    public BoundedRational ToBoundedRational() =>
        _crProperty != null && (IsOne(_crProperty) || _ratFactor.Sign == 0) ? _ratFactor : BoundedRational.Null;

    /// <summary>
    /// Returns equivalent BigInteger result if it exists, null if not.
    /// </summary>
    public BigInteger? ToBigInteger()
    {
        BoundedRational r = ToBoundedRational();
        return r.ToBigInteger();
    }

    /// <summary>
    /// Returns a suitable representation of ln(arg) or log(arg). arg is positive and not one. 
    /// kind is IsLn or IsLog.
    /// </summary>
    private static UnifiedReal LogRep(PropertyKind kind, BoundedRational arg)
    {
        if (arg.CompareToOne() < 0)
        {
            // Convert to an argument larger than one. Normalizing arguments in this way increases the
            // chance of repeat occurrences of the same argument, and makes them cleaner to display.
            return LogRep(kind, BoundedRational.Inverse(arg)).Negate();
        }

        BigInteger? intArg = arg.ToBigInteger();
        if (intArg != null)
        {
            UnifiedReal? smallPowerLog = LgSmallPower(kind, intArg.Value);
            if (smallPowerLog != null)
            {
                return smallPowerLog;
            }
        }

        if (arg.BitLength > LogArgBits)
        {
            return kind == PropertyKind.IsLn
                ? new UnifiedReal(arg.ToConstructiveReal().Ln())
                : new UnifiedReal(arg.ToConstructiveReal().Ln().Divide(CrLn10));
        }

        return new UnifiedReal(BoundedRational.One, MakeProperty(kind, arg));
    }

    /// <summary>
    /// Return x+y
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    public UnifiedReal Add(UnifiedReal u)
    {
        if (SameCrFactor(u))
        {
            BoundedRational nRatFactor = _ratFactor + u._ratFactor;
            if (!nRatFactor.IsNull)
            {
                return new UnifiedReal(nRatFactor, _crFactor, _crProperty);
            }
        }

        if (DefinitelyZero())
        {
            // Avoid creating new crFactor, even if they don't currently match.
            return u;
        }

        if (u.DefinitelyZero())
        {
            return this;
        }

        // Consider "simplifying" sums of logs.
        if (_crProperty != null && u._crProperty != null
                                && _crProperty.Kind == u._crProperty.Kind
                                && (_crProperty.Kind == PropertyKind.IsLn || _crProperty.Kind == PropertyKind.IsLog))
        {
            // a ln(b) + c ln(d) = ln(b^a * d^c)
            // a log(b) + c log(d) = log(b^a * d^c)
            // If the resulting ln argument is reasonably compact, compute the sum as the right side
            // instead, since that preserves the symbolic representation.
            BigInteger? ratAsInt = _ratFactor.ToBigInteger();
            BigInteger? uRatAsInt = u._ratFactor.ToBigInteger();
            if (ratAsInt != null && uRatAsInt != null)
            {
                double ratAsDouble = (double)ratAsInt.Value;
                double uRatAsDouble = (double)uRatAsInt.Value;
                // Estimate size of resulting argument.
                double estimatedSize = Math.Abs(ratAsDouble) * _crProperty.Arg.BitLength
                                       + Math.Abs(uRatAsDouble) * u._crProperty.Arg.BitLength;
                if (estimatedSize <= (float)LogArgCandidateBits)
                {
                    BoundedRational term1 = BoundedRational.Pow(_crProperty.Arg, _ratFactor);
                    BoundedRational term2 = BoundedRational.Pow(u._crProperty.Arg, u._ratFactor);
                    BoundedRational newArg = term1 * term2;
                    if (!newArg.IsNull)
                    {
                        return LogRep(_crProperty.Kind, newArg);
                    } // The else case here is probably impossible, since we already checked the size.
                }
            }
        }

        // Since we got here, neither ratFactor is zero.
        // We can still conclude that the result is irrational, so long as the two arguments
        // are independent. But it can be counter-productive to track this if the arguments
        // are of greatly differing magnitude. We know that 1 + e^(-e^10000) is irrational,
        // but we still don't want to evaluate it sufficiently to distinguish it from 1.
        // Thus, we want to treat 1 + e^(-e^10000) as not comparable to rationals.
        // We in fact don't track this if either argument might be ridiculously small, where
        // ridiculously small is < 10^-1000, and thus also way outside of IEEE exponent range.
        CRProperty? resultProp = null;
        if (DefinitelyIndependent(u)
            && LeadingBinaryZeroes() < -DefaultComparisonTolerance
            && u.LeadingBinaryZeroes() < -DefaultComparisonTolerance)
        {
            resultProp = MakeProperty(PropertyKind.IsIrrational, BoundedRational.Null);
        }

        return new UnifiedReal(ToConstructiveReal().Add(u.ToConstructiveReal()), resultProp);
    }

    // Don't track ln() or log() arguments whose representation is larger than this.
    private const int LogArgBits = 100;

    // Don't even attempt to simplify ln() or log() arguments larger than this.
    private const int LogArgCandidateBits = 2000;

    /// <summary>
    /// Return -x
    /// </summary>
    /// <returns></returns>
    public UnifiedReal Negate() => new(-_ratFactor, _crFactor, _crProperty);

    /// <summary>
    /// Return x-y
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    public UnifiedReal Subtract(UnifiedReal u) => Add(u.Negate());

    /// <summary>
    /// Return sqrt(x*y) as a UnifiedReal.
    /// </summary>
    private static UnifiedReal MultiplySqrts(BoundedRational x, BoundedRational y)
    {
        if (x.Equals(y))
        {
            return new UnifiedReal(x);
        }

        BoundedRational product = (x * y).Reduce();
        if (product.Sign == 0)
        {
            return Zero;
        }

        if (!product.IsNull)
        {
            var decomposedProduct = product.ExtractSquareReduced();
            return new UnifiedReal(decomposedProduct[0], decomposedProduct[1].ToConstructiveReal().Sqrt(),
                MakeProperty(PropertyKind.IsSqrt, decomposedProduct[1]));
        }

        return new UnifiedReal(x.ToConstructiveReal().Multiply(y.ToConstructiveReal()).Sqrt());
    }

    /// <summary>
    /// Return x*y
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    public UnifiedReal Multiply(UnifiedReal u)
    {
        // Preserve a preexisting crFactor when we can.
        if (_crProperty != null && IsOne(_crProperty))
        {
            BoundedRational nRatFactor1 = _ratFactor * u._ratFactor;
            if (!nRatFactor1.IsNull)
            {
                return new UnifiedReal(nRatFactor1, u._crFactor, u._crProperty);
            }
        }

        if (u._crProperty != null && IsOne(u._crProperty))
        {
            BoundedRational nRatFactor2 = _ratFactor * u._ratFactor;
            if (!nRatFactor2.IsNull)
            {
                return new UnifiedReal(nRatFactor2, _crFactor, _crProperty);
            }
        }

        if (DefinitelyZero() || u.DefinitelyZero())
        {
            return Zero;
        }

        CRProperty? resultProp = null; // Property for product of crFactors.
        BoundedRational nRatFactor = _ratFactor * u._ratFactor;
        if (_crProperty != null && u._crProperty != null)
        {
            if (_crProperty.Kind == PropertyKind.IsSqrt && u._crProperty.Kind == PropertyKind.IsSqrt)
            {
                BoundedRational sqrtArg = GetSqrtArg(_crProperty);
                BoundedRational uSqrtArg = GetSqrtArg(u._crProperty);
                UnifiedReal crPart = MultiplySqrts(sqrtArg, uSqrtArg);
                BoundedRational ratResult = nRatFactor * crPart._ratFactor;
                if (!ratResult.IsNull)
                {
                    return new UnifiedReal(ratResult, crPart._crFactor, crPart._crProperty);
                }
            }

            if (_crProperty.Kind == PropertyKind.IsExp && u._crProperty.Kind == PropertyKind.IsExp)
            {
                // exp(a) * exp(b) is exp(a + b) .
                BoundedRational sum = _crProperty.Arg + u._crProperty.Arg;
                if (!sum.IsNull)
                {
                    // we use this only for the property, since crFactors may already have been evaluated.
                    resultProp = MakeProperty(PropertyKind.IsExp, sum);
                }
            }
        }

        // Probably a bit cheaper to multiply component-wise.
        // TODO: We should often be able to determine that the result is irrational.
        // But definitelyIndependent is not the right criterion. Consider e and e^-1.
        if (!nRatFactor.IsNull)
        {
            return new UnifiedReal(nRatFactor, _crFactor.Multiply(u._crFactor), resultProp);
        }

        // resultProp invalid for this computation; discard. We know that the ratFactors are nonzero.
        return new UnifiedReal(ToConstructiveReal().Multiply(u.ToConstructiveReal()));
    }

    /// <summary>
    /// Return the reciprocal.
    /// </summary>
    /// <exception cref="DivideByZeroException"></exception>
    public UnifiedReal Inverse()
    {
        if (DefinitelyZero())
        {
            throw new DivideByZeroException("Inverse of zero");
        }

        if (_crProperty != null && IsOne(_crProperty))
        {
            return new UnifiedReal(BoundedRational.Inverse(_ratFactor));
        }

        BoundedRational square = GetSqrtArg(_crProperty);
        if (square.ToBigInteger() != null)
        {
            // Prefer square roots of integers. 1/sqrt(n) = sqrt(n)/n
            BoundedRational nRatFactor =
                BoundedRational.Inverse(_ratFactor * square);
            if (!nRatFactor.IsNull)
            {
                return new UnifiedReal(nRatFactor, _crFactor, _crProperty);
            }
        }

        CRProperty? newProperty = null;
        if (_crProperty is { Kind: PropertyKind.IsExp })
        {
            newProperty = MakeProperty(PropertyKind.IsExp, -_crProperty.Arg);
        }
        else if (DefinitelyIrrational())
        {
            newProperty = MakeProperty(PropertyKind.IsIrrational, BoundedRational.Null);
        }

        return new UnifiedReal(BoundedRational.Inverse(_ratFactor), _crFactor.Inverse(), newProperty);
    }

    /// <summary>
    /// Return x/y
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    /// <exception cref="DivideByZeroException"></exception>
    public UnifiedReal Divide(UnifiedReal u)
    {
        if (SameCrFactor(u))
        {
            if (u.DefinitelyZero())
            {
                throw new DivideByZeroException("Division by zero");
            }

            BoundedRational nRatFactor = _ratFactor / u._ratFactor;
            if (!nRatFactor.IsNull)
            {
                return new UnifiedReal(nRatFactor);
            }
        }

        // Try to reduce ln(x)/ln(10) to log(x) to keep symbolic representation.
        BoundedRational lnArg = GetLnArg(_crProperty);
        if (!lnArg.IsNull)
        {
            BoundedRational uLnArg = GetLnArg(u._crProperty);
            if (!uLnArg.IsNull && uLnArg.Equals(BoundedRational.Ten))
            {
                BoundedRational ratQuotient = _ratFactor / u._ratFactor;
                if (!ratQuotient.IsNull)
                {
                    return new UnifiedReal(ratQuotient, MakeProperty(PropertyKind.IsLog, lnArg));
                }
            }
        }

        return Multiply(u.Inverse());
    }

    /// <summary>
    /// Return the square root. This may return a value with a null property, rather than a known
    /// rational, even when the result is rational.
    /// </summary>
    public UnifiedReal Sqrt()
    {
        if (ApproxSign(DefaultComparisonTolerance) < 0)
        {
            throw new ArithmeticException("sqrt(negative)");
        }

        if (DefinitelyZero())
        {
            return Zero;
        }

        CRProperty? newCrProperty = null;
        if (_crProperty != null && IsOne(_crProperty))
        {
            BoundedRational r = _ratFactor.Reduce();
            if (r.ExtractSquareWillSucceed())
            {
                // Avoid generating IS_SQRT property for rational values.
                var decomposedProduct = r.ExtractSquareReduced();
                newCrProperty = decomposedProduct[1].CompareToOne() == 0
                    ? MakeProperty(PropertyKind.IsOne, BoundedRational.Null)
                    : MakeProperty(PropertyKind.IsSqrt, decomposedProduct[1]);

                return new UnifiedReal(decomposedProduct[0],
                    decomposedProduct[1].ToConstructiveReal().Sqrt(), newCrProperty);
            } // else don't track; we don't know if it's rational.
        }

        // If this is exp(a), result is exp(a/2). Track that.
        BoundedRational expArg = GetExpArg(_crProperty);
        if (!expArg.IsNull)
        {
            BoundedRational newArg = expArg / BoundedRational.Two;
            if (!newArg.IsNull)
            {
                newCrProperty = MakeProperty(PropertyKind.IsExp, newArg);
            }
        }

        return new UnifiedReal(ToConstructiveReal().Sqrt(), newCrProperty);
    }

    /// <summary>
    /// Return (this mod 2pi)/(pi/6) as a BigInteger, or null if that isn't easily possible.
    /// </summary>
    private BigInteger? GetPiTwelfths()
    {
        if (DefinitelyZero()) return BigInteger.Zero;
        if (_crProperty != null && IsPi(_crProperty))
        {
            BigInteger? quotient = (_ratFactor * BoundedRational.Twelve).ToBigInteger();
            if (quotient == null)
            {
                return null;
            }

            return quotient.Value % Big24;
        }

        return null;
    }

    /// <summary>
    /// Compute the sin of an integer multiple n of pi/12, if easily representable.
    /// </summary>
    /// <param name="n">value between 0 and 23 inclusive.</param>
    private static UnifiedReal? SinPiTwelfths(int n)
    {
        if (n >= 12)
        {
            UnifiedReal? negResult = SinPiTwelfths(n - 12);
            return negResult?.Negate();
        }

        return n switch
        {
            0 => Zero,
            2 => Half, // 30 degrees
            3 => HalfSqrt2, // 45 degrees
            4 => HalfSqrt3, // 60 degrees
            6 => One,
            8 => HalfSqrt3,
            9 => HalfSqrt2,
            10 => Half,
            _ => null
        };
    }

    private static UnifiedReal? CosPiTwelfths(int n)
    {
        int sinArg = n + 6;
        if (sinArg >= 24)
        {
            sinArg -= 24;
        }

        return SinPiTwelfths(sinArg);
    }

    /// <summary>
    /// Return the sine of this number.
    /// </summary>
    /// <returns></returns>
    public UnifiedReal Sin()
    {
        BigInteger? piTwelfths = GetPiTwelfths();
        if (piTwelfths != null)
        {
            UnifiedReal? result = SinPiTwelfths((int)piTwelfths.Value);
            if (result != null)
            {
                return result;
            }
        }

        if (_crProperty != null && IsPi(_crProperty))
        {
            SignedProperty? newCrProperty1 = MakeSinPiProperty(_ratFactor);
            if (newCrProperty1 != null)
            {
                return new UnifiedReal(
                    (newCrProperty1.Negative ? BoundedRational.MinusOne : BoundedRational.One),
                    newCrProperty1.Property);
            }
        }

        BoundedRational aSinArg = GetASinArg(_crProperty);
        if (!aSinArg.IsNull && _ratFactor.CompareToOne() == 0)
        {
            return new UnifiedReal(aSinArg);
        }

        CRProperty? newCrProperty = DefinitelyAlgebraic() && DefinitelyNonzero()
            ? MakeProperty(PropertyKind.IsIrrational, BoundedRational.Null)
            : null;
        return new UnifiedReal(ToConstructiveReal().Sin(), newCrProperty);
    }

    /// <summary>
    /// Return a copy of the argument that is at least marked is irrational.
    /// </summary>
    private static UnifiedReal TagIrrational(UnifiedReal arg) => arg._crProperty == null
        ? new UnifiedReal(arg._ratFactor, arg._crFactor, MakeProperty(PropertyKind.IsIrrational, BoundedRational.Null))
        : arg;

    /// <summary>
    /// Return the cosine of this number.
    /// </summary>
    /// <returns></returns>
    public UnifiedReal Cos()
    {
        if (DefinitelyAlgebraic() && DefinitelyNonzero())
        {
            // We know from Lindemann-Weierstrass that the result is transcendental, and therefore
            // irrational.
            return TagIrrational(Add(PIOver2).Sin());
        }

        return Add(PIOver2).Sin();
    }

    /// <summary>
    /// Return the tangent of this number.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArithmeticException"></exception>
    public UnifiedReal Tan()
    {
        BigInteger? piTwelfths = GetPiTwelfths();
        if (piTwelfths != null)
        {
            int i = (int)piTwelfths.Value;
            if (i is 6 or 18)
            {
                throw new DomainException("Tangent undefined");
            }

            UnifiedReal? top = SinPiTwelfths(i);
            UnifiedReal? bottom = CosPiTwelfths(i);
            if (top != null && bottom != null)
            {
                return top.Divide(bottom);
            }
        }

        if (_crProperty != null && IsPi(_crProperty))
        {
            SignedProperty? newCrProperty1 = MakeTanPiProperty(_ratFactor);
            if (newCrProperty1 != null)
            {
                return new UnifiedReal(
                    newCrProperty1.Negative ? BoundedRational.MinusOne : BoundedRational.One,
                    newCrProperty1.Property);
            }
        }

        BoundedRational aTanArg = GetATanArg(_crProperty);
        if (!aTanArg.IsNull && _ratFactor.CompareToOne() == 0)
        {
            return new UnifiedReal(aTanArg);
        }

        CRProperty? newCrProperty = DefinitelyAlgebraic() && DefinitelyNonzero()
            ? MakeProperty(PropertyKind.IsIrrational, BoundedRational.Null)
            : null;
        return new UnifiedReal(UnaryCRFunction.TanFunction.Execute(ToConstructiveReal()), newCrProperty);
    }

    // Throw an exception if the argument is definitely out of bounds for asin or acos.
    private void CheckAsinDomain()
    {
        if (IsComparable(One) && (CompareTo(One) > 0 || CompareTo(MinusOne) < 0))
        {
            throw new DomainException("inverse trig argument out of range");
        }
    }

    /// <summary>
    /// Return asin(n/2). n is between -2 and 2.
    /// </summary>
    public static UnifiedReal AsinHalves(int n) =>
        n switch
        {
            < 0 => (AsinHalves(-n).Negate()),
            0 => Zero,
            1 => new UnifiedReal(BoundedRational.Sixth, CrPI),
            2 => new UnifiedReal(BoundedRational.Half, CrPI),
            _ => throw new ArgumentOutOfRangeException(nameof(n))
        };

    /// <summary>
    /// Return the arcsine of this number.
    /// </summary>
    /// <returns></returns>
    public UnifiedReal Asin()
    {
        CheckAsinDomain();
        BigInteger? halves = Multiply(Two).ToBigInteger();
        if (halves != null)
        {
            return AsinHalves((int)halves.Value);
        }

        if (CompareTo(Zero, -10) < 0)
        {
            return Negate().Asin().Negate();
        }

        if (DefinitelyEquals(HalfSqrt2))
        {
            return new UnifiedReal(BoundedRational.Quarter, CrPI);
        }

        if (DefinitelyEquals(HalfSqrt3))
        {
            return new UnifiedReal(BoundedRational.Third, CrPI);
        }

        BoundedRational sinPiArg = GetSinPiArg(_crProperty);
        if (!sinPiArg.IsNull)
        {
            if (_ratFactor.CompareToOne() == 0)
            {
                return new UnifiedReal(sinPiArg, CrPI);
            }

            if (_ratFactor.CompareTo(BoundedRational.MinusOne) == 0)
            {
                return new UnifiedReal(-sinPiArg, CrPI, CRProperty.PropPI);
            }
        }

        if (_crProperty != null && IsOne(_crProperty))
        {
            Check(_ratFactor.Sign > 0);
            return new UnifiedReal(MakeProperty(PropertyKind.IsASin, _ratFactor));
        }

        return new UnifiedReal(ToConstructiveReal().Asin());
    }

    /// <summary>
    /// Return the arccosine of this number.
    /// </summary>
    /// <returns></returns>
    public UnifiedReal Acos()
    {
        return PIOver2.Subtract(Asin());
    }

    /// <summary>
    /// Return the arctangent of this number.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public UnifiedReal Atan()
    {
        if (CompareTo(Zero, -10) < 0)
        {
            return Negate().Atan().Negate();
        }

        BigInteger? asBigInt = ToBigInteger();
        if (asBigInt != null && asBigInt.Value.CompareTo(BigInteger.One) <= 0)
        {
            int asInt = (int)asBigInt.Value;
            // These seem to be all rational cases:
            switch (asInt)
            {
                case 0:
                    return Zero;
                case 1:
                    return PIOver4;
                default:
                    throw new InvalidOperationException("Impossible r_int");
            }
        }

        if (DefinitelyEquals(ThirdSqrt3))
        {
            return PIOver6;
        }

        if (DefinitelyEquals(Sqrt3))
        {
            return PIOver3;
        }

        BoundedRational tanPiArg = GetTanPiArg(_crProperty);
        if (!tanPiArg.IsNull)
        {
            if (_ratFactor.CompareToOne() == 0)
            {
                return new UnifiedReal(tanPiArg, CrPI);
            }

            if (_ratFactor.CompareTo(BoundedRational.MinusOne) == 0)
            {
                return new UnifiedReal(-tanPiArg, CrPI);
            }
        }

        if (_crProperty != null && IsOne(_crProperty))
        {
            Check(_ratFactor.Sign > 0);
            return new UnifiedReal(MakeProperty(PropertyKind.IsATan, _ratFactor));
        }

        return new UnifiedReal(UnaryCRFunction.AtanFunction.Execute(ToConstructiveReal()));
    }

    /// <summary>
    /// Compute an integral power of a constructive real, using the standard recursive algorithm. exp
    /// is known to be positive.
    /// </summary>
    private static ConstructiveReal RecursivePow(ConstructiveReal @base, BigInteger exp)
    {
        if (exp == BigInteger.One)
        {
            return @base;
        }

        if ((exp & BigInteger.One) != BigInteger.Zero) // exp.testBit(0)
        {
            return @base.Multiply(RecursivePow(@base, exp - BigInteger.One));
        }

        ConstructiveReal tmp = RecursivePow(@base, exp >> 1);

        return tmp.Multiply(tmp);
    }

    /// <summary>
    /// Compute an integral power of a constructive real, using the exp function when we safely can.
    /// Use RecursivePow when we can't. exp is known to be nonzero.
    /// </summary>
    private UnifiedReal ExpLnPow(BigInteger exp)
    {
        int sign = ApproxSign(DefaultComparisonTolerance);
        if (sign > 0)
        {
            // Safe to take the log. This avoids deep recursion for huge exponents, which
            // may actually make sense here.
            return new UnifiedReal(ToConstructiveReal().Ln().Multiply(ConstructiveReal.FromBigInteger(exp)).Exp());
        }

        if (sign < 0)
        {
            ConstructiveReal result = ToConstructiveReal().Negate().Ln().Multiply(ConstructiveReal.FromBigInteger(exp))
                .Exp();
            if ((exp & BigInteger.One) != BigInteger.Zero) /* odd exponent */
            {
                result = result.Negate();
            }

            return new UnifiedReal(result);
        }

        // Base of unknown sign with integer exponent. Use a recursive computation.
        // (Another possible option would be to use the absolute value of the base, and then
        // adjust the sign at the end. But that would have to be done in the CR
        // implementation.)
        if (exp.Sign < 0)
        {
            // This may be very expensive if exp.negate() is large.
            return new UnifiedReal(RecursivePow(ToConstructiveReal(), -exp).Inverse());
        }

        return new UnifiedReal(RecursivePow(ToConstructiveReal(), exp));
    }

    /// <summary>
    /// Compute an integral power of this. This recurses roughly as deeply as the number of bits in the
    /// exponent, and can, in ridiculous cases, result in a stack overflow.
    /// </summary>
    /// <exception cref="ArithmeticException"></exception>
    private UnifiedReal Pow(BigInteger exp)
    {
        if (exp == BigInteger.One)
        {
            return this;
        }

        int expSign = exp.Sign;
        if (expSign == 0)
        {
            // The following check may diverge, causing us to time out. This only happens
            // if we try to raise something that is zero, but not obviously so, to the
            // zeroth power.
            if (DefinitelySign() != 0)
            {
                return One;
            }

            // Base is known to be exactly zero.
            throw new ZeroToTheZerothException("0^0");
        }

        if (DefinitelyZero() && expSign < 0)
        {
            throw new ArithmeticException("zero to negative power");
        }

        BigInteger absExp = BigInteger.Abs(exp);
        if (_crProperty != null && IsOne(_crProperty))
        {
            double resultLen = (double)exp * _ratFactor.ApproxLog2Abs();
            // Both multiplicands may be negative. That still implies a huge answer.
            if (resultLen > BitLimit /* +INFINITY qualifies */)
            {
                throw new TooBigException("Power result is too big");
            }

            if (absExp.CompareTo(HardRecursivePowLimit) <= 0)
            {
                BoundedRational ratPow = _ratFactor.Pow(exp);
                // We count on this to fail, e.g. for very large exponents, when it would
                // otherwise be too expensive.
                if (!ratPow.IsNull)
                {
                    return new UnifiedReal(ratPow);
                }
            }
        }

        if (absExp.CompareTo(RecursivePowLimit) > 0)
        {
            return ExpLnPow(exp);
        }

        BoundedRational square = GetSqrtArg(_crProperty);
        if (!square.IsNull)
        {
            // Compute powers as UnifiedReals, so we get the limit checking above.
            UnifiedReal resultFactor1 = new UnifiedReal(_ratFactor).Pow(exp);
            UnifiedReal squareAsUReal = new UnifiedReal(square);
            UnifiedReal resultFactor2 = squareAsUReal.Pow(exp >> 1);
            UnifiedReal product = resultFactor1.Multiply(resultFactor2);
            if ((exp & BigInteger.One) == BigInteger.One)
            {
                // Odd power: Multiply by remaining square root.
                return product.Multiply(squareAsUReal.Sqrt());
            }

            return product;
        }

        return ExpLnPow(exp);
    }

    /// <summary>
    /// Return this ^ exp. This is really only well-defined for a positive base, particularly since
    /// 0^x is not continuous at zero. 0^0 = 1 (as is epsilon^0), but 0^epsilon is 0. We nonetheless
    /// try to do reasonable things at zero, when we recognize that case.
    /// </summary>
    public UnifiedReal Pow(UnifiedReal exp)
    {
        if (_crProperty != null)
        {
            if (_crProperty.Kind == PropertyKind.IsExp && _crProperty.Arg.CompareToOne() == 0)
            {
                if (_ratFactor.Equals(BoundedRational.One))
                {
                    return exp.Exp();
                }

                // (<ratFactor>e)^<exp> = <ratFactor>^<exp> * e^<exp>
                UnifiedReal ratPart = new UnifiedReal(_ratFactor).Pow(exp);
                return exp.Exp().Multiply(ratPart);
            }

            if (_crProperty.Kind == PropertyKind.IsOne && _ratFactor.CompareTo(BoundedRational.Ten) == 0)
            {
                BoundedRational expLogArg = GetLogArg(exp._crProperty);
                if (!expLogArg.IsNull)
                {
                    // 10^(r * log(expLogArg)) = expLogArg^r
                    return new UnifiedReal(expLogArg).Pow(new UnifiedReal(exp._ratFactor));
                }
            }
        }

        int sign = ApproxSign(DefaultComparisonTolerance);
        BoundedRational expAsBr = exp.ToBoundedRational();
        bool knownIrrational = false;
        if (!expAsBr.IsNull)
        {
            var expNumDen = expAsBr.NumDen;
            if (expNumDen.Item2 == BigInteger.One)
            {
                return Pow(expNumDen.Item1);
            }

            // Check for the case in which both arguments are rational, and there is an
            // exact rational answer.
            // We explicitly avoid returning a result for a negative base here,
            // even when that would make sense, as in (-8)^(1/3).
            // This is probably wrong if we're computing cube roots.
            // But note that we could never return a meaningful result for
            // (-8)^<1/3 computed so we can't recognize it as such>.
            if (sign >= 0 && _crProperty != null && IsOne(_crProperty) && expNumDen.Item2.GetBitLength() <= 30)
            {
                int expDen = (int)expNumDen.Item2; // Doesn't lose information.
                // Don't just use BoundedRational.pow(), since that would bypass above checks.
                BoundedRational rt = BoundedRational.NthRoot(_ratFactor, expDen);
                if (!rt.IsNull)
                {
                    return new UnifiedReal(rt).Pow(expNumDen.Item1);
                }

                // We know that the root is irrational. Raising it to a power relatively prime to expDen
                // is not going to change that.
                knownIrrational = true;
            }

            // Explicitly check for the square root case, in case the result is representable
            // as an integer multiple of a small square root.
            if (expNumDen.Item2 == BigTwo)
            {
                return Pow(expNumDen.Item1).Sqrt();
            }
        }

        // If the exponent were known zero, we would have handled it above.
        if (sign == 0 && DefinitelyZero())
        {
            // Compute the exponent sign, at the risk of divergence. The result depends on it.
            int expSign = exp.DefinitelySign();
            if (expSign > 0)
            {
                return Zero;
            }

            if (expSign < 0)
            {
                throw new ArithmeticException("zero to negative power");
            }

            // Unclear we can get here.
            throw new ZeroToTheZerothException("0^0");
        }

        if (sign < 0)
        {
            throw new ArithmeticException("Negative base for pow() with non-integer exponent");
        }

        if (knownIrrational)
        {
            return new UnifiedReal(ToConstructiveReal().Ln().Multiply(exp.ToConstructiveReal()).Exp(),
                MakeProperty(PropertyKind.IsIrrational, BoundedRational.Null));
        }

        return new UnifiedReal(ToConstructiveReal().Ln().Multiply(exp.ToConstructiveReal()).Exp());
    }

    /// <summary>
    /// Return the integral log with respect to the given base if it exists, 0 otherwise. n is presumed
    /// positive. base is presumed to be at least 2.
    /// </summary>
    private static long GetIntLog(BigInteger n, int @base)
    {
        double nAsDouble = (double)n;
        double approx = Math.Log(nAsDouble) / Math.Log(@base);
        // A relatively quick test first.
        // Try something else for values to big for a double.
        if (double.IsInfinity(nAsDouble))
        {
            // Floating point test doesn't help. Try another quick test.
            if (@base % 2 != 0 && !n.IsEven)
            {
                // Has a divisor of 2. Can't be a power of an odd number.
                return 0;
            }

            if (@base % 3 != 0 && n % BigThree == 0)
            {
                return 0;
            }

            if (@base % 5 != 0 && n % BigFive == 0)
            {
                return 0;
            }
        }
        else if (Math.Abs(approx - Math.Round(approx)) > 1.0e-6)
        {
            return 0;
        }

        // It's important to avoid allocating large numbers of large BigIntegers here.
        // In particular, we need to avoid e.g. repeatedly dividing by base.
        // Otherwise, computations like log(100,000!) can behave very badly.
        // This algorithm performs worst case O(log(log n)) BigInteger operations.
        // We build a set of powers of base by repeated squaring, with powers[i] == base^(2^i).
        long result = 0;
        var powers = new List<BigInteger> { new BigInteger(@base) };
        BigInteger nReduced = n; // always equal to n/base^result .
        for (int i = 1;; ++i)
        {
            BigInteger last = powers[i - 1];
            BigInteger next = last * last; // base^(2^i)
            if (next.GetBitLength() > nReduced.GetBitLength())
            {
                break;
            }

            BigInteger quotient = BigInteger.DivRem(nReduced, next, out var remainder);
            if (remainder != 0)
            {
                // A power of base smaller than 2 * nReduced didn't divide nReduced.
                // nReduced, and thus n, are clearly not a power of base.
                return 0;
            }

            powers.Add(next);
            // Since we have the quotient, opportunistically reduce n.
            result += (1L << i);
            nReduced = quotient;
        }

        // We've computed all repeated squaring powers <= nReduced.
        // Use those to divide repeatedly until we get to one, or determine it's not
        // a power of base.
        for (int i = powers.Count - 1; nReduced != BigInteger.One; --i)
        {
            BigInteger power = powers[i];
            if (power.GetBitLength() <= nReduced.GetBitLength())
            {
                BigInteger quotient = BigInteger.DivRem(nReduced, power, out var remainder);
                if (remainder != 0)
                {
                    return 0;
                }

                result += (1L << i);
                nReduced = quotient;
                // Now power.GetBitLength() > nReduced.GetBitLength() .
                // Otherwise we would have divided by the next bigger power, which is power^2.
            }
        }

        return result;
    }

    /// <summary>
    /// If the argument is a positive power of 10, return its base 10 logarithm. Otherwise, return 0.
    /// We assume r strictly greater than 1.
    /// </summary>
    private static long IntLog10(BoundedRational r)
    {
        BigInteger? rAsInt = r.ToBigInteger();
        return rAsInt is not { Sign: > 0 } ? 0 : GetIntLog(rAsInt.Value, 10);
    }

    /// <summary>
    /// Return lg(n) as k lg(m) if there is a small integer m such that n = m^k. Lg is either
    /// log or ln, depending on the kind argument. Return null if there are no such m and k.
    /// </summary>
    private static UnifiedReal? LgSmallPower(PropertyKind kind, BigInteger n)
    {
        foreach (int m in SmallNonPowers)
        {
            long intLog = GetIntLog(n, m);
            if (intLog != 0)
            {
                ConstructiveReal newCrValue;
                if (kind == PropertyKind.IsLog)
                {
                    if (m == 10)
                    {
                        return new UnifiedReal(new BoundedRational(intLog));
                    }

                    newCrValue = ConstructiveReal.FromInt(m).Ln().Divide(CrLn10);
                }
                else
                {
                    newCrValue = ConstructiveReal.FromInt(m).Ln();
                }

                return new UnifiedReal(new BoundedRational(intLog), newCrValue,
                    MakeProperty(kind, BoundedRational.FromLong(m)));
            }
        }

        return null;
    }

    // Small integers for which we try to recognize ln(small_int^n), so we can simplify it to
    // n*ln(small_int).
    private static readonly int[] SmallNonPowers = [2, 3, 5, 6, 7, 10];

    /// <summary>
    /// Natural logarithm 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArithmeticException"></exception>
    public UnifiedReal Ln()
    {
        BoundedRational expArg = GetExpArg(_crProperty);
        CRProperty? newCrProperty = null;
        if (!expArg.IsNull)
        {
            return new UnifiedReal(_ratFactor).Ln().Add(new UnifiedReal(expArg));
        }

        int sign = ApproxSign(DefaultComparisonTolerance);
        if (sign < 0)
        {
            throw new DomainException("log(negative)");
        }

        if (IsComparable(Zero))
        {
            if (sign == 0)
            {
                throw new DomainException("log(zero)");
            }

            int compare1 = CompareTo(One, DefaultComparisonTolerance);
            if (compare1 == 0)
            {
                if (DefinitelyEquals(One))
                {
                    return Zero;
                }
            }
            else if (compare1 < 0)
            {
                return Inverse().Ln().Negate();
            }

            BigInteger? bi = _ratFactor.ToBigInteger();
            if (bi != null)
            {
                if (_crProperty != null && IsOne(_crProperty))
                {
                    UnifiedReal? smallPowerLn = LgSmallPower(PropertyKind.IsLn, bi.Value);
                    if (smallPowerLn != null)
                    {
                        return smallPowerLn;
                    }
                }
                else
                {
                    // Check for n^k * sqrt(n), for which we can also return a more useful answer.
                    BigInteger? square = GetSqrtArg(_crProperty).ToBigInteger();
                    if (square != null && square.Value.GetBitLength() < 30)
                    {
                        int intSquare = (int)square.Value;
                        long intLog = GetIntLog(bi.Value, intSquare);
                        if (intLog != 0)
                        {
                            BoundedRational nRatFactor =
                                BoundedRational.FromLong(intLog) + BoundedRational.Half;
                            if (!nRatFactor.IsNull)
                            {
                                return new UnifiedReal(nRatFactor, ConstructiveReal.FromBigInteger(square.Value).Ln(),
                                    MakeProperty(PropertyKind.IsLn, new BoundedRational(square.Value)));
                            }
                        }
                    }
                }
            }

            if (_crProperty != null && IsOne(_crProperty))
            {
                // Normalize to argument > 1, and remember symbolic representation.
                return LogRep(PropertyKind.IsLn, _ratFactor);
            }
        }

        return new UnifiedReal(ToConstructiveReal().Ln(), newCrProperty);
    }

    /// <summary>
    /// Base 10 logarithm
    /// </summary>
    /// <exception cref="ArithmeticException"></exception>
    public UnifiedReal Log()
    {
        return Ln().Divide(Ln10);
    }

    /// <summary>
    /// Computes the exponential function e^this.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArithmeticException"></exception>
    public UnifiedReal Exp()
    {
        if (DefinitelyEquals(Zero))
        {
            return One;
        }

        if (DefinitelyEquals(One))
        {
            // Avoid redundant computations, and ensure we recognize all instances as equal.
            return E;
        }

        BoundedRational lnArg = GetLnArg(_crProperty);
        if (!lnArg.IsNull)
        {
            bool needSqrt = false;
            BoundedRational ratExponent = _ratFactor;
            BigInteger? asBigInt = ratExponent.ToBigInteger();
            if (asBigInt == null)
            {
                // check for multiple of one half.
                needSqrt = true;
                ratExponent = ratExponent * BoundedRational.Two;
            }

            BoundedRational nRatFactor = BoundedRational.Pow(lnArg, ratExponent);
            if (!nRatFactor.IsNull)
            {
                UnifiedReal result = new UnifiedReal(nRatFactor);
                if (needSqrt)
                {
                    result = result.Sqrt();
                }

                return result;
            }
        }

        if (CompareTo(BitLimitAsUReal, 0) > 0)
        {
            throw new TooBigException("exp argument is too big");
        }

        CRProperty? newCrProperty = null;
        if (_crProperty != null && IsOne(_crProperty))
        {
            newCrProperty = MakeProperty(PropertyKind.IsExp, _ratFactor);
        }

        return new UnifiedReal(ToConstructiveReal().Exp(), newCrProperty);
    }

    /// <summary>
    /// Absolute value.
    /// </summary>
    public UnifiedReal Abs()
    {
        if (IsComparable(Zero))
        {
            return DefinitelySign() < 0 ? Negate() : this;
        }

        return new UnifiedReal(ToConstructiveReal().Abs(),
            _crProperty != null && IsUnknownIrrational(_crProperty)
                ? MakeProperty(PropertyKind.IsIrrational, BoundedRational.Null)
                : null);
    }

    /// <summary>
    /// Generalized factorial. Compute n * (n - step) * (n - 2 * step) * etc. This can be used to
    /// compute factorial a bit faster, especially if BigInteger uses sub-quadratic multiplication.
    /// </summary>
    private static BigInteger GenFactorial(long n, long step)
    {
        if (n > 4 * step)
        {
            BigInteger prod1 = GenFactorial(n, 2 * step);
            BigInteger prod2 = GenFactorial(n - step, 2 * step);

            return prod1 * prod2;
        }

        if (n == 0)
        {
            return BigInteger.One;
        }

        BigInteger res = new BigInteger(n);
        for (long i = n - step; i > 1; i -= step)
        {
            res = res * new BigInteger(i);
        }

        return res;
    }

    /// <summary>
    /// Factorial function. Fails if argument is clearly not an integer. May round to nearest integer
    /// if value is close.
    /// </summary>
    public UnifiedReal Fact()
    {
        BigInteger? asBigInt = ToBigInteger();
        if (asBigInt == null)
        {
            asBigInt = ToConstructiveReal().GetApproximation(0); // Correct if it was an integer.
            if (!ApproxEquals(new UnifiedReal(asBigInt.Value), DefaultComparisonTolerance))
            {
                throw new DomainException("Non-integral factorial argument");
            }
        }

        if (asBigInt.Value.Sign < 0)
        {
            throw new DomainException("Negative factorial argument");
        }

        if (asBigInt.Value.GetBitLength() > 18)
        {
            // Several million digits. Will fail.  LongValue() may not work. Punt now.
            throw new TooBigException("Factorial argument too big");
        }

        BigInteger biResult = GenFactorial((long)asBigInt.Value, 1);
        BoundedRational nRatFactor = new BoundedRational(biResult);
        return new UnifiedReal(nRatFactor);
    }

    /// <summary>
    /// Returns an n such that the absolute value of the associated constructive real is no less
    /// than 2^n. n may be negative. Integer.MIN_VALUE indicates we didn't find a bound.
    /// </summary>
    private static int MsbBound(CRProperty? p)
    {
        if (p == null)
        {
            return int.MinValue;
        }

        switch (p.Kind)
        {
            case PropertyKind.IsOne:
                return 0;
            case PropertyKind.IsPi:
                return 1;
            case PropertyKind.IsSqrt:
                int wnb = p.Arg.WholeNumberBits;
                if (wnb == int.MinValue)
                {
                    return wnb;
                }

                return (p.Arg.WholeNumberBits >> 1) - 2;
            case PropertyKind.IsLn:
            case PropertyKind.IsLog:
                if (p.Arg.CompareTo(BoundedRational.Two) >= 0)
                {
                    // ln(2) > log(2) > 1/4
                    return -2;
                }

                // Argument is in the vicinity of 1, result may be near zero.
                return int.MinValue;
            case PropertyKind.IsExp:
                BigInteger result = p.Arg.Floor();
                int sign = result.Sign;
                if (result.GetBitLength() <= 30)
                {
                    if (sign >= 0)
                    {
                        // multiply by a bit less than 1/ln(2).
                        return ((int)result / 5) * 7;
                    }

                    // multiply by a bit more than 1/ln(2), making sure we err on correct side.
                    return ((int)result / 2 - 1) * 3;
                }

                if (sign > 0)
                {
                    // Positive and takes > 30 bits to represent.
                    return 100_000_000;
                }

                return int.MinValue;
            case PropertyKind.IsSinPi:
            case PropertyKind.IsTanPi:
            case PropertyKind.IsASin:
            case PropertyKind.IsATan:
                // These all behave like x or <pi>x near zero. Thus, the following very rough estimate holds.
                if (p.Arg.CompareTo(Br1Over1024) >= 1)
                {
                    return -11;
                }

                return int.MinValue;
            case PropertyKind.IsIrrational:
                return int.MinValue;
            default:
                throw new InvalidOperationException("Unexpected PropertyKind"); // Can't get here.
        }
    }

    private static readonly BoundedRational Br1Over1024 = new BoundedRational(1, 1024);

    /// <summary>
    /// Return the number of decimal digits to the right of the decimal point required to represent
    /// the argument exactly. Return Integer.MAX_VALUE if that's not possible. Never returns a value
    /// less than zero, even if r is a power of ten.
    /// </summary>
    public int DigitsRequired()
    {
        if (_crProperty != null && (IsOne(_crProperty) || _ratFactor.Sign == 0))
        {
            return _ratFactor.DigitsRequired;
        }

        return int.MaxValue;
    }

    /// <summary>
    /// Return an upper bound on the number of leading zero bits. These are the number of 0 bits to the
    /// right of the binary point and to the left of the most significant digit. Return
    /// Integer.MAX_VALUE if we cannot bound it based only on the rational factor and property.
    /// </summary>
    public int LeadingBinaryZeroes()
    {
        int crBound = MsbBound(_crProperty); // lower bound on binary log.
        if (crBound != int.MinValue)
        {
            int wholeBits = _ratFactor.WholeNumberBits;
            if (wholeBits == int.MinValue)
            {
                return int.MaxValue;
            }

            if (wholeBits + crBound >= 3)
            {
                return 0;
            }

            return -(wholeBits + crBound) + 3;
        }

        return int.MaxValue;
    }

    /// <summary>
    /// Is the number of bits to the left of the decimal point greater than bound? The result is
    /// inexact: We roughly approximate the whole number bits. bound is non-negative.
    /// </summary>
    public bool ApproxWholeNumberBitsGreaterThan(int bound)
    {
        Check(bound >= 0);
        int crBound = MsbBound(_crProperty);
        int ratBits = _ratFactor.WholeNumberBits;
        if (crBound != int.MinValue && ratBits != int.MinValue)
        {
            return ratBits + crBound > bound;
        }

        return ToConstructiveReal().GetApproximation(bound - 2).GetBitLength() > 2;
    }
}