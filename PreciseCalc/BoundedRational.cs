using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace PreciseCalc;

/// <summary>
///     Represents rational numbers that may become null if they get too big.
/// </summary>
/// <remarks>
///     <para>
///         For many operations, if the length of the numerator plus the length of the denominator
///         exceeds a maximum size, we simply return null, and rely on our caller do something else.
///         We currently never return null for a pure integer or for a BoundedRational that has just been constructed.
///     </para>
///     <para>
///         We also implement a number of irrational functions.
///         These return a non-null result only when the result is known to be rational.
///     </para>
/// </remarks>
public readonly struct BoundedRational : IEquatable<BoundedRational>, IComparable<BoundedRational>,
    IAdditionOperators<BoundedRational, BoundedRational, BoundedRational>,
    ISubtractionOperators<BoundedRational, BoundedRational, BoundedRational>,
    IMultiplyOperators<BoundedRational, BoundedRational, BoundedRational>,
    IDivisionOperators<BoundedRational, BoundedRational, BoundedRational>
{
    // TODO: Consider returning null for integers.  With some care, large factorials might become much faster.
    // TODO: Maybe eventually make this extend Number?

    private const int MaxSize = 10000; // total, in bits

    // Max bit length for attempting to extract square, so as not to take too much time.
    // Large enough so that computations on floating point numbers cannot easily overflow this.
    private const int ExtractSquareMaxLen = 5000;
    private static readonly Random ReduceRng = new();

    /// <summary>
    ///     Max integer for which <see cref="ExtractSquare" /> is guaranteed to be optimal.
    /// </summary>
    /// <remarks>
    ///     We currently fail to do so for 44 = 11*4, but succeed for all perfect squares*n, with n &lt;= 10
    ///     and numerator and denominator size &lt; <see cref="ExtractSquareMaxLen" />.
    /// </remarks>
    public const int ExtractSquareMaxOpt = 43;

    #region Constants

    /// <summary>
    ///     Singleton for zero.
    /// </summary>
    public static readonly BoundedRational Zero = new(0);

    /// <summary>
    ///     Singleton for one half.
    /// </summary>
    public static readonly BoundedRational Half = new(1, 2);

    /// <summary>
    ///     Singleton for minus one half.
    /// </summary>
    public static readonly BoundedRational MinusHalf = new(-1, 2);

    /// <summary>
    ///     Singleton for one third.
    /// </summary>
    public static readonly BoundedRational Third = new(1, 3);

    /// <summary>
    ///     Singleton for one fourth.
    /// </summary>
    public static readonly BoundedRational Quarter = new(1, 4);

    /// <summary>
    ///     Singleton for one sixth.
    /// </summary>
    public static readonly BoundedRational Sixth = new(1, 6);

    /// <summary>
    ///     Singleton for one.
    /// </summary>
    public static readonly BoundedRational One = new(1);

    /// <summary>
    ///     Singleton for minus one.
    /// </summary>
    public static readonly BoundedRational MinusOne = new(-1);

    /// <summary>
    ///     Singleton for two.
    /// </summary>
    public static readonly BoundedRational Two = new(2);

    /// <summary>
    ///     Singleton for minus two.
    /// </summary>
    public static readonly BoundedRational MinusTwo = new(-2);

    /// <summary>
    ///     Singleton for three.
    /// </summary>
    public static readonly BoundedRational Three = new(3);

    /// <summary>
    ///     Singleton for ten.
    /// </summary>
    public static readonly BoundedRational Ten = new(10);

    /// <summary>
    ///     Singleton for twelve.
    /// </summary>
    public static readonly BoundedRational Twelve = new(12);

    /// <summary>
    ///     Singleton for an invalid bounded rational.
    /// </summary>
    public static readonly BoundedRational Null = new();

    #endregion

    private readonly BigInteger _numerator;
    private readonly BigInteger _denominator;

    #region Constructors

    /// <summary>
    ///     Create an invalid bounded rational.
    /// </summary>
    public BoundedRational()
    {
        _numerator = default;
        _denominator = default;
        HasValue = false;
    }

    /// <summary>
    ///     Create a valid bounded rational.
    /// </summary>
    /// <param name="numerator"></param>
    /// <param name="denominator"></param>
    public BoundedRational(BigInteger numerator, BigInteger denominator)
    {
        _numerator = numerator;
        _denominator = denominator;
        HasValue = true;
    }

    /// <summary>
    ///     Create a valid bounded rational.
    /// </summary>
    /// <param name="value"></param>
    public BoundedRational(BigInteger value) : this(value, BigInteger.One)
    {
    }

    /// <summary>
    ///     Create a valid bounded rational.
    /// </summary>
    /// <param name="numerator"></param>
    /// <param name="denominator"></param>
    public BoundedRational(long numerator, long denominator) : this(new BigInteger(numerator),
        new BigInteger(denominator))
    {
    }

    /// <summary>
    ///     Create a valid bounded rational.
    /// </summary>
    /// <param name="value"></param>
    public BoundedRational(long value) : this(new BigInteger(value), BigInteger.One)
    {
    }

    /// <summary>
    ///     Produce BoundedRational equal to the given double.
    /// </summary>
    /// <exception cref="ArithmeticException">Infinity or Nan</exception>
    public static BoundedRational FromDouble(double value)
    {
        var longValue = Math.Round(value);
        if (Math.Abs(longValue - value) < double.Epsilon && Math.Abs(longValue) <= 1000)
            return new BoundedRational((long)longValue);

        var allBits = BitConverter.DoubleToUInt64Bits(Math.Abs(value));
        var mantissa = allBits & ((1UL << 52) - 1UL);
        var biasedExp = (int)(allBits >> 52);

        if ((biasedExp & 0x7ff) == 0x7ff)
            throw new ArithmeticException("Infinity or NaN not convertible to BoundedRational");

        long sign = value < 0.0 ? -1 : 1;
        var exp = biasedExp - 1075; // 1023 + 52; we treat mantissa as integer

        if (biasedExp == 0)
            exp += 1; // Denormal exponent is 1 greater
        else
            mantissa += 1UL << 52; // Implied leading one

        var num = new BigInteger(sign * (long)mantissa);
        var den = BigInteger.One;

        if (exp >= 0)
            num <<= exp;
        else
            den <<= -exp;

        return new BoundedRational(num, den);
    }

    /// <summary>
    ///     Produce BoundedRational equal to the given long.
    /// </summary>
    public static BoundedRational FromLong(long x)
    {
        return x switch
        {
            -2L => MinusTwo,
            -1L => MinusOne,
            0L => Zero,
            1L => One,
            2L => Two,
            10L => Ten,
            _ => new BoundedRational(x)
        };
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Returns true if the bounded rational is valid.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    ///     Returns the sign of this rational number.
    /// </summary>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    public int Sign => HasValue
        ? _numerator.Sign * _denominator.Sign
        : throw new InvalidOperationException("Invalid bounded rational.");

    /// <summary>
    ///     Returns the pair of numerator and denominator.
    /// </summary>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    public (BigInteger numerator, BigInteger denominator) NumDen
    {
        get
        {
            if (!HasValue) throw new InvalidOperationException("Invalid bounded rational.");

            var nicer = Reduce().WithPositiveDenominator();
            return (nicer._numerator, nicer._denominator);
        }
    }

    /// <summary>
    ///     Computes the number of decimal digits required for exact representation.
    /// </summary>
    /// <remarks>
    ///     Return the number of decimal digits to the right of the decimal point
    ///     required to represent the argument exactly.
    ///     Return <see cref="int.MaxValue" /> if that's not possible.
    ///     Never returns a value less than zero, even if r is a power of ten.
    /// </remarks>
    public int DigitsRequired
    {
        get
        {
            if (!HasValue) return int.MaxValue;

            var powersOfTwo = 0;
            var powersOfFive = 0;

            if (_denominator == BigInteger.One) return 0;

            var r = Reduce();
            var den = r._denominator;

            if (den.GetBitLength() > MaxSize) return int.MaxValue;

            while (den.IsEven)
            {
                powersOfTwo++;
                den >>= 1;
            }

            var five = new BigInteger(5);
            while (den % five == BigInteger.Zero)
            {
                powersOfFive++;
                den /= five;
            }

            if (den != BigInteger.One && den != BigInteger.MinusOne) return int.MaxValue;

            return Math.Max(powersOfTwo, powersOfFive);
        }
    }

    /// <summary>
    ///     Number of bits in the representation. Makes the most sense for the result of Reduce(),
    ///     since it does not implicitly reduce.
    /// </summary>
    /// <remarks>
    ///     &lt;= MaxSize
    /// </remarks>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    public int BitLength =>
        HasValue
            ? (int)(_numerator.GetBitLength() + _denominator.GetBitLength())
            : throw new InvalidOperationException("Invalid bounded rational.");

    /// <summary>
    ///     Approximate number of bits to left of binary point.
    ///     Negative indicates leading zeroes to the right of binary point.
    /// </summary>
    /// <remarks>
    ///     &lt;= MaxSize
    /// </remarks>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    public int WholeNumberBits => HasValue
        ? _numerator.Sign == 0 ? int.MinValue : (int)(_numerator.GetBitLength() - _denominator.GetBitLength())
        : throw new InvalidOperationException("Invalid bounded rational.");

    #endregion

    #region ToString variants

    /// <summary>
    ///     Converts to a string representation.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return HasValue ? $"{_numerator}/{_denominator}" : "Null";
    }

    private const char FractionSlash = '\u2044';
    private const char SuperscriptMinus = '\u207B';

    /// <summary>
    ///     Converts to a readable string representation. Renamed from toNiceString.
    /// </summary>
    /// <remarks>
    ///     Intended for output to user. More expensive, less useful for debugging than ToString().
    ///     <paramref name="useUnicodeFractions" /> relies on Unicode characters intended for this purpose.
    ///     Not internationalized.
    /// </remarks>
    /// <param name="useUnicodeFractions">
    ///     If true, use subscript and superscript characters to represent the fraction.
    /// </param>
    /// <param name="useMixedNumbers">
    ///     If true, convert improper fractions to mixed fractions.
    /// </param>
    public string ToDisplayString(bool useUnicodeFractions = false, bool useMixedNumbers = false)
    {
        if (!HasValue) return "Null";

        var nicer = Reduce().WithPositiveDenominator();
        var num = BigInteger.Abs(nicer._numerator);
        var den = nicer._denominator;
        var isNegative = nicer._numerator.Sign < 0;
        BigInteger? whole = null;

        if (nicer._denominator.Equals(BigInteger.One))
        {
            whole = num;
            num = BigInteger.Zero;
        }
        else if (useMixedNumbers && num >= den)
        {
            (whole, num) = BigInteger.DivRem(num, den);
        }

        var result = new StringBuilder();
        if (isNegative) result.Append(useUnicodeFractions ? SuperscriptMinus : '-');

        if (whole.HasValue) result.Append(whole.Value);

        if (num.IsZero) return result.ToString();

        if (whole.HasValue) result.Append(' ');

        if (useUnicodeFractions)
            result.Append(num).Append(FractionSlash).Append(den);
        else
            result.Append(num).Append('/').Append(den);

        return result.ToString();
    }

    /// <summary>
    ///     Truncate the rational representation to specified digits.
    /// </summary>
    /// <param name="n">Result precision, >= 0</param>
    public string ToStringTruncated(int n)
    {
        if (!HasValue) return "Null";

        var digits = (BigInteger.Abs(_numerator) * BigInteger.Pow(10, n) /
                      BigInteger.Abs(_denominator)).ToString(CultureInfo.InvariantCulture);

        var len = digits.Length;
        if (len < n + 1)
        {
            digits = new string('0', n + 1 - len) + digits;
            len = n + 1;
        }

        return (Sign < 0 ? "-" : "") +
               digits.Substring(0, len - n) +
               "." +
               digits.Substring(len - n);
    }

    #endregion

    #region Converters

    /// <summary>
    ///     Return a double approximation.
    /// </summary>
    /// <remarks>
    ///     The result is correctly rounded to nearest, with ties rounded away from zero.
    ///     TODO: Should round ties to even.
    /// </remarks>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    public double ToDouble()
    {
        if (!HasValue) throw new InvalidOperationException("Invalid bounded rational.");

        var sign = Sign;
        if (sign < 0) return -(-this).ToDouble();

        var approxExp = (int)_numerator.GetBitLength() - (int)_denominator.GetBitLength();
        if (approxExp < -1100 || sign == 0) return 0.0;

        var neededPrecision = approxExp - 80;
        var dividend = neededPrecision < 0 ? _numerator << -neededPrecision : _numerator;
        var divisor = neededPrecision > 0 ? _denominator << neededPrecision : _denominator;
        var quotient = BigInteger.Divide(dividend, divisor);
        var qLength = (int)quotient.GetBitLength();
        var extraBits = qLength - 53;
        var exponent = neededPrecision + qLength;

        if (exponent >= -1021)
        {
            --exponent;
        }
        else
        {
            extraBits += -1022 - exponent + 1;
            exponent = -1023;
        }

        var bigMantissa = (quotient + (BigInteger.One << (extraBits - 1))) >> extraBits;

        if (exponent > 1024) return double.PositiveInfinity;

        var mantissa = (long)bigMantissa;
        var bits = (mantissa & ((1L << 52) - 1)) | ((exponent + 1023L) << 52);
        return BitConverter.Int64BitsToDouble(bits);
    }

    /// <summary>
    ///     Returns the constructive real representation of this rational number.
    /// </summary>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    public ConstructiveReal ToConstructiveReal()
    {
        return HasValue
            ? ConstructiveReal.FromBigInteger(_numerator) / ConstructiveReal.FromBigInteger(_denominator)
            : throw new InvalidOperationException("Invalid bounded rational.");
    }

    /// <summary>
    ///     Returns the integer value of this rational number.
    ///     Throws an exception if this is not an integer.
    /// </summary>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    /// <exception cref="ArithmeticException">IntValue of non-int</exception>
    public int ToInt32()
    {
        if (!HasValue) throw new InvalidOperationException("Invalid bounded rational.");

        var reduced = Reduce();
        if (!reduced._denominator.Equals(BigInteger.One)) throw new ArithmeticException("IntValue of non-int");

        return (int)reduced._numerator;
    }

    /// <summary>
    ///     Returns equivalent BigInteger if it exists, null if not.
    /// </summary>
    public BigInteger? ToBigInteger()
    {
        if (!HasValue) return null;

        var (quotient, remainder) = BigInteger.DivRem(_numerator, _denominator);
        return remainder == BigInteger.Zero ? quotient : null;
    }

    /// <summary>
    ///     Returns an approximation of the base 2 log of the absolute value.
    ///     We assume this is nonzero.
    ///     The result is either 0 or within 20% of the truth.
    /// </summary>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    public double ApproxLog2Abs()
    {
        if (!HasValue) throw new InvalidOperationException("Invalid bounded rational.");

        var wholeBits = WholeNumberBits;
        if (wholeBits > 10 || wholeBits < -10)
            // Bit lengths suffice for our purposes.
            return wholeBits;

        // Argument is in the vicinity of one. Numerator and denominator are nonzero,
        // but may be individually huge.
        var quotient = Math.Abs((double)_numerator / (double)_denominator);
        if (double.IsInfinity(quotient) || double.IsNaN(quotient) || quotient == 0.0)
            // Zero quotient means denominator overflowed and is meaningless. Ignore.
            return 0.0;

        return Math.Log(quotient) / Math.Log(2.0);
    }

    #endregion

    # region Recuders

    /// <summary>
    ///     Returns whether this number is too big for continued rational arithmetic.
    /// </summary>
    /// <remarks>
    ///     Is this number too big for us to continue with rational arithmetic?
    ///     We return false for integers on the assumption that we have no better fallback.
    /// </remarks>
    private bool IsTooBig()
    {
        return !_denominator.Equals(BigInteger.One) &&
               _numerator.GetBitLength() + _denominator.GetBitLength() > MaxSize;
    }

    /// <summary>
    ///     Returns an equivalent fraction with a positive denominator.
    /// </summary>
    private BoundedRational WithPositiveDenominator()
    {
        if (!HasValue) return Null;

        if (_denominator.Sign > 0) return this;

        return new BoundedRational(-_numerator, -_denominator);
    }

    /// <summary>
    ///     Reduces this fraction to the lowest terms.
    /// </summary>
    /// <remarks>
    ///     Denominator sign may remain negative.
    /// </remarks>
    public BoundedRational Reduce()
    {
        if (!HasValue) return Null;

        if (_denominator.Equals(BigInteger.One)) return this;

        var gcd = BigInteger.GreatestCommonDivisor(_numerator, _denominator);
        return new BoundedRational(_numerator / gcd, _denominator / gcd);
    }

    private static BoundedRational MaybeReduce(BoundedRational r)
    {
        if (!r.HasValue) return Null;

        if (!r.IsTooBig() && (ReduceRng.Next() & 0xf) != 0) return r;

        var result = r.WithPositiveDenominator().Reduce();
        return !result.IsTooBig() ? result : Null;
    }

    #endregion

    #region Interface implementations

    /// <summary>
    ///     Compares this bounded rational to another.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(BoundedRational other)
    {
        return (HasValue, other.HasValue) switch
        {
            (true, true) => (_numerator * other._denominator)
                            .CompareTo(other._numerator * _denominator)
                            * _denominator.Sign
                            * other._denominator.Sign,
            (true, false) => 1,
            (false, true) => -1,
            _ => 0
        };
    }

    /// <summary>
    ///     Equivalent to CompareTo(BoundedRational.One) but faster.
    /// </summary>
    public int CompareToOne()
    {
        return HasValue ? _numerator.CompareTo(_denominator) * _denominator.Sign : -1;
    }

    /// <summary>
    ///     Compares this bounded rational to another.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(BoundedRational other)
    {
        return CompareTo(other) == 0;
    }

    /// <summary>
    ///     Compares this bounded rational to another.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is BoundedRational other && Equals(other);
    }

    /// <summary>
    ///     Returns the hash code of this bounded rational.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        if (!HasValue) return 0;

        var reduced = Reduce().WithPositiveDenominator();
        return HashCode.Combine(reduced._numerator, reduced._denominator);
    }

    /// <summary>
    ///     Compares two bounded rationals.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(BoundedRational left, BoundedRational right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two bounded rationals.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(BoundedRational left, BoundedRational right)
    {
        return !(left == right);
    }

    #endregion

    #region Arithmetic operator overloads

    /// <summary>
    ///     Adds two bounded rationals.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BoundedRational operator +(BoundedRational left, BoundedRational right)
    {
        if (!left.HasValue || !right.HasValue) return Null;

        var den = left._denominator * right._denominator;
        var num = left._numerator * right._denominator + right._numerator * left._denominator;
        return MaybeReduce(new BoundedRational(num, den));
    }

    /// <summary>
    ///     Negates a bounded rational.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static BoundedRational operator -(BoundedRational value)
    {
        return !value.HasValue ? Null : new BoundedRational(-value._numerator, value._denominator);
    }

    /// <summary>
    ///     Subtracts two bounded rationals.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BoundedRational operator -(BoundedRational left, BoundedRational right)
    {
        return left + -right;
    }

    /// <summary>
    ///     Multiplies two bounded rationals.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BoundedRational operator *(BoundedRational left, BoundedRational right)
    {
        return MaybeReduce(RawMultiply(left, right));
    }

    /// <summary>
    ///     Divides two bounded rationals.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BoundedRational operator /(BoundedRational left, BoundedRational right)
    {
        return left * Inverse(right);
    }

    #endregion

    #region Comparison operator overloads

    /// <summary>
    ///     Returns a value that indicates whether a BoundedRational value is less than another BoundedRational value.
    /// </summary>
    public static bool operator <(BoundedRational left, BoundedRational right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    ///     Returns a value that indicates whether a BoundedRational value is greater than another BoundedRational value.
    /// </summary>
    public static bool operator >(BoundedRational left, BoundedRational right)
    {
        return 0 < left.CompareTo(right);
    }

    /// <summary>
    ///     Returns a value that indicates whether a BoundedRational value is less than or equal to another BoundedRational
    ///     value.
    /// </summary>
    public static bool operator <=(BoundedRational left, BoundedRational right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    ///     Returns a value that indicates whether a BoundedRational value is greater than or equal to another BoundedRational
    ///     value.
    /// </summary>
    public static bool operator >=(BoundedRational left, BoundedRational right)
    {
        return 0 <= left.CompareTo(right);
    }

    #endregion

    #region Other arithmetic operations

    internal static BoundedRational RawMultiply(BoundedRational left, BoundedRational right)
    {
        if (!left.HasValue || !right.HasValue) return Null;

        // Optimize multiplication by one
        if (left.Equals(One)) return right;

        if (right.Equals(One)) return left;

        var num = left._numerator * right._numerator;
        var den = left._denominator * right._denominator;
        return new BoundedRational(num, den);
    }

    /// <summary>
    ///     Returns the inverse of a bounded rational.
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    /// <exception cref="DivideByZeroException"></exception>
    public static BoundedRational Inverse(BoundedRational r)
    {
        if (!r.HasValue) return Null;

        if (r._numerator.IsZero) throw new DivideByZeroException();

        return new BoundedRational(r._denominator, r._numerator);
    }

    /// <summary>
    ///     Returns closest integer no larger than this.
    /// </summary>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    public BigInteger Floor()
    {
        if (!HasValue) throw new InvalidOperationException("Invalid bounded rational.");

        var (quotient, remainder) = BigInteger.DivRem(_numerator, _denominator);
        var result = quotient;

        if (remainder.Sign < 0) result -= BigInteger.One;

        return result;
    }

    /// <summary>
    ///     Returns the nth root if it exists and is rational, null otherwise.
    /// </summary>
    /// <exception cref="ArithmeticException">even root(negative)</exception>
    private static BigInteger? NthRoot(BigInteger x, int n)
    {
        var sign = x.Sign;
        if (sign < 0)
        {
            if ((n & 1) == 0) throw new ArithmeticException("even root(negative)");

            return -NthRoot(-x, n);
        }

        if (sign == 0) return BigInteger.Zero;

        var xAsCr = ConstructiveReal.FromBigInteger(x);
        ConstructiveReal rootAsCr;

        if (n == 2)
            rootAsCr = xAsCr.Sqrt();
        else if (n == 4)
            rootAsCr = xAsCr.Sqrt().Sqrt();
        else
            rootAsCr = (xAsCr.Ln() / ConstructiveReal.FromInt(n)).Exp();

        const int scale = -10;
        var scaledRoot = rootAsCr.GetApproximation(scale);
        const int fracMask = (1 << -scale) - 1;
        var fracBits = (int)(scaledRoot & fracMask);

        if (fracBits != 0 && fracBits != fracMask) return null;

        BigInteger rootAsBigInt;
        if (fracBits == 0)
            rootAsBigInt = scaledRoot >> -scale;
        else
            rootAsBigInt = (scaledRoot + 1) >> -scale;

        if (BigInteger.Pow(rootAsBigInt, n) == x) return rootAsBigInt;

        return null;
    }

    /// <summary>
    ///     Compute nth root exactly. Returns null if irrational.
    /// </summary>
    public static BoundedRational NthRoot(BoundedRational r, int n)
    {
        if (!r.HasValue) return Null;

        if (n < 0)
        {
            var invRoot = NthRoot(r, -n);
            return !invRoot.HasValue ? Null : Inverse(invRoot);
        }

        r = r.WithPositiveDenominator().Reduce();
        var numRt = NthRoot(r._numerator, n);
        var denRt = NthRoot(r._denominator, n);

        return numRt == null || denRt == null ? Null : new BoundedRational(numRt.Value, denRt.Value);
    }

    /// <summary>
    ///     Returns the square root if rational, null otherwise.
    /// </summary>
    public static BoundedRational Sqrt(BoundedRational r)
    {
        return NthRoot(r, 2);
    }

    private static readonly BigInteger[] SomePrimes =
    [
        new(2), new(3), new(5),
        new(7), new(11), new(13)
    ];

    private static readonly BigInteger[] PrimeSquares =
    [
        new(4), new(9), new(25),
        new(49), new(121), new(169)
    ];

    /// <summary>
    ///     Returns pair p, where p[0]^2 * p[1] = x. Tries to maximize p[0].
    /// </summary>
    private static BigInteger[] ExtractSquare(BigInteger x)
    {
        var square = BigInteger.One;
        var rest = x;

        if (rest.GetBitLength() > ExtractSquareMaxLen) return [square, rest];

        for (var i = 0; i < SomePrimes.Length; i++)
        {
            if (rest == BigInteger.One) break;

            while (true)
            {
                var divRem = BigInteger.DivRem(rest, PrimeSquares[i]);
                if (divRem.Remainder == BigInteger.Zero)
                {
                    rest = divRem.Quotient;
                    square *= SomePrimes[i];
                }
                else
                {
                    break;
                }
            }
        }

        // Check whether rest/<small int> is a perfect square
        for (var i = 1; i <= 10; i++)
        {
            var divRem = BigInteger.DivRem(rest, new BigInteger(i));
            if (divRem.Remainder == BigInteger.Zero)
            {
                var root = NthRoot(divRem.Quotient, 2);
                if (root != null)
                {
                    rest = new BigInteger(i);
                    square *= root.Value;
                    break;
                }
            }
        }

        return [square, rest];
    }

    /// <summary>
    ///     Returns a pair p such that p[0]^2 * p[1] = this.
    ///     Tries to maximize p[0]. This rational is assumed to be in reduced form.
    /// </summary>
    /// <exception cref="InvalidOperationException">When invalid</exception>
    public BoundedRational[] ExtractSquareReduced()
    {
        if (!HasValue) throw new InvalidOperationException("Invalid bounded rational.");

        if (Sign == 0) return [Zero, One];

        var numResult = ExtractSquare(BigInteger.Abs(_numerator));
        var denResult = ExtractSquare(BigInteger.Abs(_denominator));

        if (Sign < 0) numResult[1] = -numResult[1];

        return
        [
            new BoundedRational(numResult[0], denResult[0]),
            new BoundedRational(numResult[1], denResult[1])
        ];
    }

    /// <summary>
    ///     Will ExtractSquareReduced guarantee that p[1] is not a perfect square?
    ///     This rational is assumed to be in reduced form.
    /// </summary>
    public bool ExtractSquareWillSucceed()
    {
        return HasValue &&
               _numerator.GetBitLength() < ExtractSquareMaxLen &&
               _denominator.GetBitLength() < ExtractSquareMaxLen;
    }

    /// <summary>
    ///     Compute integral power, assuming this has been reduced and exp >= 0.
    /// </summary>
    private BoundedRational RawPow(BigInteger exp)
    {
        if (exp == BigInteger.One) return this;

        if (!exp.IsEven) return RawMultiply(RawPow(exp - BigInteger.One), this);

        if (exp == BigInteger.Zero) return One;

        var tmp = RawPow(exp >> 1);

        var result = RawMultiply(tmp, tmp);
        if (!result.HasValue || result.IsTooBig()) return Null;

        return result;
    }

    /// <summary>
    ///     Compute an integral power of this rational.
    /// </summary>
    public BoundedRational Pow(BigInteger exp)
    {
        var expSign = exp.Sign;
        if (expSign == 0)
            // Questionable if base has undefined or zero value.
            // System.Math.Pow() returns 1 anyway, so we do the same.
            return One;

        if (exp == BigInteger.One) return this;

        // Reducing once at the beginning means there's no point in reducing later.
        var reduced = Reduce().WithPositiveDenominator();

        // First handle cases in which huge exponents could give compact results.
        if (reduced._denominator == BigInteger.One)
        {
            if (reduced._numerator == BigInteger.Zero) return Zero;

            if (reduced._numerator == BigInteger.One) return One;

            if (reduced._numerator == new BigInteger(-1)) return !exp.IsEven ? MinusOne : One;
        }

        if (exp.GetBitLength() > 1000)
            // Stack overflow is likely; a useful rational result is not.
            return Null;

        return expSign < 0 ? Inverse(reduced).RawPow(-exp) : reduced.RawPow(exp);
    }

    /// <summary>
    ///     Computes r^exp
    /// </summary>
    public static BoundedRational Pow(BoundedRational r, BoundedRational exp)
    {
        if (!exp.HasValue || !r.HasValue) return Null;

        exp = exp.Reduce().WithPositiveDenominator();
        if (exp._denominator.GetBitLength() > 30) return Null;

        var expDen = (int)exp._denominator; // Doesn't lose information due to bit length check
        if (expDen == 1) return r.Pow(exp._numerator);

        var rt = NthRoot(r, expDen);
        return !rt.HasValue ? Null : rt.Pow(exp._numerator);
    }

    #endregion
}