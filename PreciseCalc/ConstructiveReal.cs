using System.Globalization;
using System.Numerics;

namespace PreciseCalc;

/// <summary>
/// Represents a constructive real number, allowing arbitrary-precision calculations.
/// </summary>
public abstract class ConstructiveReal
{
    // First some frequently used constants, so we don't have to
    // recompute these all over the place.

    internal static readonly BigInteger Big0 = BigInteger.Zero;
    internal static readonly BigInteger Big1 = BigInteger.One;
    internal static readonly BigInteger BigM1 = BigInteger.MinusOne;
    internal static readonly BigInteger Big2 = new BigInteger(2);
    internal static readonly BigInteger BigM2 = new BigInteger(-2);
    internal static readonly BigInteger Big3 = new BigInteger(3);
    internal static readonly BigInteger Big6 = new BigInteger(6);
    internal static readonly BigInteger Big8 = new BigInteger(8);
    internal static readonly BigInteger Big10 = new BigInteger(10);
    internal static readonly BigInteger Big750 = new BigInteger(750);
    internal static readonly BigInteger BigM750 = new BigInteger(-750);

    // Volatile flag for interruption handling

    /// <summary>
    /// Setting this to true requests that all computations be aborted by
    /// throwing AbortedException.
    /// Must be rest to false before any further
    /// computation.
    /// </summary>
    public static volatile bool PleaseStop = false;

    /// <summary>
    /// Subclasses must implement this method to approximate the value to a given precision.
    /// </summary>
    /// <remarks>
    /// Most users can ignore the existence of this method, and will
    /// not ever need to define a subclass of <see cref="ConstructiveReal"/>.
    /// Returns value / 2 ** <paramref name="precision"/> rounded to an integer.
    /// The error in the result is strictly &lt; 1.
    /// Informally, Approximate(n) gives a scaled approximation
    /// accurate to 2**n.
    /// Implementations may safely assume that <paramref name="precision"/> is
    /// at least a factor of 8 away from overflow.
    /// Called only with the lock on the <see cref="ConstructiveReal"/> object
    /// already held.
    /// </remarks>
    private protected abstract BigInteger Approximate(int precision);

    // Caching fields (mutable state)

    /// <summary>The smallest precision value with which the above has been called.</summary>
    private protected int MinPrec;

    /// <summary>The scaled approximation corresponding to <see cref="MinPrec"/>.</summary>
    private protected BigInteger MaxAppr;

    /// <summary>The scaled approximation corresponding to <see cref="MinPrec"/>.</summary>
    private protected bool ApprValid;

    /// <summary>
    /// Computes the logarithm base 2 of an integer, rounded up.
    /// </summary>
    internal static int BoundLog2(int n)
    {
        int absN = Math.Abs(n);
        return (int)Math.Ceiling(Math.Log(absN + 1) / Math.Log(2.0));
    }

    /// <summary>
    /// Check that a precision is at least a factor of 8 away from
    /// overflowing the integer used to hold a precision spec.
    /// We generally perform this check early on, and then convince
    /// ourselves that none of the operations performed on precisions
    /// inside a function can generate an overflow.
    /// </summary>
    internal static void CheckPrecision(int precision)
    {
        int high = precision >> 28;
        // if precision is not in danger of overflowing, then the 4 high order
        // bits should be identical.  Thus, high is either 0 or -1.
        // The rest of this is to test for either of those in a way
        // that should be as cheap as possible.
        int highShifted = precision >> 29;
        if ((high ^ highShifted) != 0)
        {
            throw new PrecisionOverflowException();
        }
    }

    /// <summary>
    /// Creates a constructive real number from a <see cref="BigInteger"/>.
    /// </summary>
    public static ConstructiveReal FromBigInteger(BigInteger value) => new IntConstructiveReal(value);

    /// <summary>
    /// Creates a constructive real number from an <see cref="int"/>.
    /// </summary>
    public static ConstructiveReal FromInt(int value) => FromBigInteger(new BigInteger(value));

    /// <summary>
    /// Creates a constructive real number from a <see cref="long"/>.
    /// </summary>
    public static ConstructiveReal FromLong(long value) => FromBigInteger(new BigInteger(value));

    /// <summary>
    /// Creates a constructive real number from a <see cref="double"/>.
    /// </summary>
    /// <remarks>
    /// The result is undefined if argument is infinite or NaN.
    /// </remarks>
    public static ConstructiveReal FromDouble(double value)
    {
        if (double.IsNaN(value)) throw new ArithmeticException("NaN argument");
        if (double.IsInfinity(value)) throw new ArithmeticException("Infinite argument");

        bool negative = value < 0.0;
        long bits = BitConverter.DoubleToInt64Bits(Math.Abs(value));
        long mantissa = bits & 0x000FFFFFFFFFFFFF;
        int biasedExp = (int)(bits >> 52);
        int exponent = biasedExp - 1075;

        if (biasedExp != 0)
            mantissa += 1L << 52;
        else
            mantissa <<= 1;

        ConstructiveReal result = FromBigInteger(mantissa) << exponent;
        return negative ? -result : result;
    }

    /// <summary>
    /// The constructive real number corresponding to a <see cref="float"/>.
    /// </summary>
    /// <remarks>
    /// The result is undefined if argument is infinite or NaN.
    /// </remarks>
    public static ConstructiveReal FromFloat(float value) => FromDouble(value);

    // Predefined constants

    /// <summary>
    /// Predefined constant for the value 0.
    /// </summary>
    public static readonly ConstructiveReal Zero = FromInt(0);

    /// <summary>
    /// Predefined constant for the value 1.
    /// </summary>
    public static readonly ConstructiveReal One = FromInt(1);

    /// <summary>
    /// Multiply by 2**<paramref name="n"/>, rounding result
    /// </summary>
    internal static BigInteger Scale(BigInteger value, int n)
    {
        return n switch
        {
            >= 0 => value << n,
            _ => (value >> (-n - 1)) + Big1 >> 1
        };
    }

    /// <summary>
    /// Returns the approximation of the value scaled by 2^<paramref name="precision"/>, rounded to an integer.
    /// The error in the result is strictly &lt; 1.
    /// </summary>
    /// <remarks>
    /// Produces the same answer as <see cref="Approximate"/>, but uses and
    /// maintains a cached approximation.
    /// Normally not overridden, and called only from <see cref="Approximate"/>
    /// methods in subclasses.  Not needed if the provided operations
    /// on constructive reals suffice.
    /// </remarks>
    public virtual BigInteger GetApproximation(int precision)
    {
        CheckPrecision(precision);
        if (ApprValid && precision >= MinPrec)
        {
            return Scale(MaxAppr, MinPrec - precision);
        }
        else
        {
            BigInteger result = Approximate(precision);
            MinPrec = precision;
            MaxAppr = result;
            ApprValid = true;
            return result;
        }
    }

    /// <summary>
    /// Returns the most significant digit (MSD) position based on cached approximation values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <c>x.GetMsd()</c> == n then 2**(n-1) &lt; abs(x) &lt; 2**(n+1)
    /// </para>
    /// <para>
    /// This initial version assumes  that <see cref="MaxAppr"/> is valid
    /// and sufficiently removed from zero that the msd is determined.
    /// </para>
    /// </remarks>
    /// <returns>The position of the msd.</returns>
    internal int GetKnownMsd()
    {
        int length = MaxAppr.Sign >= 0 ? (int)MaxAppr.GetBitLength() : (int)(-MaxAppr).GetBitLength();
        return MinPrec + length - 1;
    }

    /// <summary>
    /// Returns the most significant digit (MSD) at the specified precision.
    /// </summary>
    /// <param name="precision">The precision at which to compute the MSD.</param>
    /// <returns>
    /// The position of the msd, or
    /// <see cref="int.MinValue"/> if the correct answer is &lt; <paramref name="precision"/>.
    /// </returns>
    internal int GetMsd(int precision)
    {
        if (!ApprValid ||
            (MaxAppr.CompareTo(Big1) <= 0 && MaxAppr.CompareTo(BigM1) >= 0))
        {
            // Compute a new approximation and update MaxAppr.
            GetApproximation(precision - 1);
            if (BigInteger.Abs(MaxAppr).CompareTo(Big1) <= 0)
            {
                // MSD could still be arbitrarily far to the right.
                return int.MinValue;
            }
        }

        return GetKnownMsd();
    }

    /// <summary>
    /// Functionally equivalent to <see cref="GetMsd(int)"/>, 
    /// but iteratively evaluates to higher precision.
    /// </summary>
    /// <param name="initialPrecision">The starting precision for refinement.</param>
    /// <returns>The most significant digit (MSD) position.</returns>
    /// <exception cref="OperationCanceledException">Thrown if computation is manually stopped.</exception>
    internal int RefineMsd(int initialPrecision)
    {
        int precision = 0;

        while (precision > initialPrecision + 30)
        {
            precision = precision * 3 / 2 - 16;
            int msd = GetMsd(precision);
            if (msd != int.MinValue)
                return msd;

            CheckPrecision(precision);

            if (PleaseStop)
            {
                throw new OperationCanceledException("Computation was manually stopped.");
            }
        }

        return GetMsd(initialPrecision);
    }

    /// <summary>
    /// Computes the most significant digit (MSD) with an iterative refinement approach.
    /// </summary>
    /// <remarks>
    /// This method will eventually return the correct answer but may loop indefinitely 
    /// (or throw an exception if precision overflows) if the value is zero.
    /// </remarks>
    /// <returns>The most significant digit of the value.</returns>
    internal int GetMsd() => RefineMsd(int.MinValue);

    /// <summary>
    /// Natural log of 2. Needed for some pre-scaling below.
    /// </summary>
    /// <remarks>
    /// ln(2) = 7ln(10/9) - 2ln(25/24) + 3ln(81/80)
    /// </remarks>
    private ConstructiveReal SimpleLn() => new PrescaledLnConstructiveReal(this - One);

    private static ConstructiveReal CalculateLn2()
    {
        ConstructiveReal tenNinths = FromInt(10) / FromInt(9);
        ConstructiveReal firstTerm = FromInt(7) * tenNinths.SimpleLn();

        ConstructiveReal twentyFiveTwentyFourths = FromInt(25) / FromInt(24);
        ConstructiveReal secondTerm = FromInt(2) * twentyFiveTwentyFourths.SimpleLn();

        ConstructiveReal eightyOneEightieths = FromInt(81) / FromInt(80);
        ConstructiveReal thirdTerm = FromInt(3) * eightyOneEightieths.SimpleLn();
        // ln(2) = 7ln(10/9) - 2ln(25/24) + 3ln(81/80)
        return firstTerm - secondTerm + thirdTerm;
    }

    // Natural log of 2.  Needed for some pre-scaling below.
    private static readonly ConstructiveReal Ln2 = CalculateLn2();

    /// <summary>
    /// Computes the arctangent of the reciprocal of an integer.
    /// </summary>
    /// <remarks>
    /// Atan of integer reciprocal.  Used for <see cref="AtanPI"/>.  Could perhaps be made public. 
    /// </remarks>
    internal static ConstructiveReal AtanReciprocal(int n) => new IntegralAtanConstructiveReal(n);

    // used for AtanPI computation.
    private static readonly ConstructiveReal Four = FromInt(4);

    // Public operations.    

    /// <summary>
    /// Compares this value with another constructive real, refining precision iteratively.
    /// </summary>
    /// <remarks>
    /// Return 0 if x = y to within the indicated tolerance,
    /// -1 if x &lt; y, and +1 if x &gt; y.  If x and y are indeed
    /// equal, it is guaranteed that 0 will be returned.  If
    /// they differ by less than the tolerance, anything
    /// may happen.  The tolerance allowed is the maximum of
    /// (Abs(this)+Abs(x))*(2**<paramref name="relPrecision"/>) and 2**<paramref name="absPrecision"/>.
    /// </remarks>
    /// <param name="x">The other constructive real</param>
    /// <param name="relPrecision">Relative tolerance in bits</param>
    /// <param name="absPrecision">Absolute tolerance in bits</param>
    public int CompareTo(ConstructiveReal x, int relPrecision, int absPrecision)
    {
        int thisMsd = RefineMsd(absPrecision);
        int xMsd = x.RefineMsd(Math.Max(thisMsd, absPrecision));
        int maxMsd = Math.Max(thisMsd, xMsd);

        if (maxMsd == int.MinValue)
        {
            return 0;
        }

        CheckPrecision(relPrecision);
        int rel = maxMsd + relPrecision;
        int abs = Math.Max(rel, absPrecision);
        return CompareTo(x, abs);
    }

    /// <summary>
    /// Compares this value with another constructive real at a given precision.
    /// </summary>
    /// <remarks>
    /// Approximate comparison with only an absolute tolerance.
    /// Identical to the three argument version, but without a relative
    /// tolerance.
    /// Result is 0 if both constructive reals are equal, indeterminate
    /// if they differ by less than 2**<paramref name="absPrecision"/> .
    /// </remarks>
    public int CompareTo(ConstructiveReal x, int absPrecision)
    {
        int neededPrecision = absPrecision - 1;
        BigInteger thisApprox = GetApproximation(neededPrecision);
        BigInteger xApprox = x.GetApproximation(neededPrecision);
        int comp1 = thisApprox.CompareTo(xApprox + Big1);
        if (comp1 > 0) return 1;
        int comp2 = thisApprox.CompareTo(xApprox - Big1);
        if (comp2 < 0) return -1;
        return 0;
    }

    /// <summary>
    /// Compares this value with another constructive real.
    /// </summary>
    /// <remarks>
    /// Return -1 if <c>this &lt; x</c>, or +1 if <c>this &gt; x</c>.
    /// Should be called only if <c>this != x</c>.
    /// If <c>this == x</c>, this will not terminate correctly; typically it
    /// will run until it exhausts memory.
    /// If the two constructive reals may be equal, the two or 3 argument
    /// version of <c>CompareTo</c> should be used.
    /// </remarks>
    public int CompareTo(ConstructiveReal x)
    {
        for (int precision = -20;; precision *= 2)
        {
            CheckPrecision(precision);
            int result = CompareTo(x, precision);
            if (result != 0) return result;
            if (PleaseStop) throw new OperationCanceledException("Computation was manually stopped.");
        }
    }

    /// <summary>
    /// Determines the sign of the value at a given precision.
    /// </summary>
    /// <remarks>
    /// Equivalent to <c>CompareTo(ConstructiveReal.FromInt(0), precision)</c>
    /// </remarks>
    public int Sign(int precision)
    {
        if (ApprValid)
        {
            int quickTry = MaxAppr.Sign;
            if (quickTry != 0) return quickTry;
        }

        int neededPrecision = precision - 1;
        BigInteger thisApprox = GetApproximation(neededPrecision);
        return thisApprox.Sign;
    }

    /// <summary>
    /// Determines the sign of the value, refining precision iteratively.
    /// </summary>
    /// <remarks>
    /// Return <c>-1</c> if negative, <c>+1</c> if positive.
    /// Should be called only if <c>this != 0</c>.
    /// In the <c>0</c> case, this will not terminate correctly; typically it
    /// will run until it exhausts memory.
    /// If the two constructive reals may be equal, the one or two argument
    /// version of Sign should be used.
    /// </remarks>
    public int Sign()
    {
        for (int precision = -20;; precision *= 2)
        {
            CheckPrecision(precision);
            int result = Sign(precision);
            if (result != 0) return result;
            if (PleaseStop) throw new OperationCanceledException("Computation was manually stopped.");
        }
    }

    /// <summary>
    /// Creates a constructive real number from a string representation in the given radix.
    /// </summary>
    public static ConstructiveReal FromString(string value, int radix)
    {
        NumberStyles style = radix switch
        {
            2 => NumberStyles.AllowBinarySpecifier,
            10 => NumberStyles.AllowLeadingSign,
            16 => NumberStyles.AllowHexSpecifier,
            _ => throw new ArgumentException("Only 2, 10, and 16 are supported.", nameof(radix))
        };

        ReadOnlySpan<char> valueSpan = value.AsSpan().Trim();
        int pointPos = valueSpan.IndexOf('.');
        ReadOnlySpan<char> fraction = pointPos == -1 ? "0".AsSpan() : valueSpan.Slice(pointPos + 1);
        ReadOnlySpan<char> whole = pointPos == -1 ? valueSpan : valueSpan.Slice(0, pointPos);
        char[] resultArray = new char[whole.Length + fraction.Length];
        whole.CopyTo(resultArray);
        fraction.CopyTo(resultArray.AsSpan(whole.Length));
        BigInteger scaledResult = BigInteger.Parse(resultArray, style);
        BigInteger divisor = BigInteger.Pow(radix, fraction.Length);
        return FromBigInteger(scaledResult) / FromBigInteger(divisor);
    }

    /// <summary>
    /// Converts the number to a string representation with the given precision and radix.
    /// </summary>
    /// <remarks>
    /// Return a textual representation accurate to <paramref name="n"/>
    /// places to the right of the decimal point.
    /// <paramref name="n"/> must be nonnegative.
    /// </remarks>
    /// <param name="n">Number of digits (&gt;= 0) included to the right of decimal point</param>
    /// <param name="radix">Base ( &gt;= 2, &lt;= 16) for the resulting representation.</param>
    public string ToString(int n, int radix)
    {
        ConstructiveReal scaledCr;
        if (radix == 16)
        {
            scaledCr = this << (4 * n);
        }
        else
        {
            BigInteger scaleFactor = BigInteger.Pow(radix, n);
            scaledCr = this * FromBigInteger(scaleFactor);
        }

        BigInteger scaledInt = scaledCr.GetApproximation(0);
        string scaledString = BigInteger.Abs(scaledInt).ToString(radix);
        string result;

        if (n == 0)
        {
            result = scaledString;
        }
        else
        {
            int len = scaledString.Length;
            if (len <= n)
            {
                // Add sufficient leading zeroes
                scaledString = new string('0', n + 1 - len) + scaledString;
                len = n + 1;
            }

            string whole = scaledString.Substring(0, len - n);
            string fraction = scaledString.Substring(len - n);
            result = whole + "." + fraction;
        }

        if (scaledInt.Sign < 0)
        {
            result = "-" + result;
        }

        return result;
    }

    /// <summary>
    /// Converts the number to a decimal string with the given precision.
    /// </summary>
    /// <remarks>
    /// Equivalent to <c>ToString(n, 10)</c>
    /// </remarks>
    /// <param name="n">Number of digits included to the right of decimal point</param>
    public string ToString(int n) => ToString(n, 10);

    /// <summary>
    /// Converts the number to a decimal string with default precision.
    /// </summary>
    /// <remarks>
    /// Equivalent to <c>ToString(10, 10)</c>
    /// </remarks>
    public override string ToString() => ToString(10);

    /// <summary>
    /// Returns a textual scientific notation representation.
    /// </summary>
    /// <remarks>
    /// Return a textual scientific notation representation
    /// accurate to <paramref name="precision"/> places to the right of the decimal point.
    /// <paramref name="precision"/> must be nonnegative.
    /// A value smaller than <paramref name="radix"/> ** -<paramref name="minPrecision"/> may be displayed as 0.
    /// The <c>Mantissa</c> component of the result is either "0" or exactly <paramref name="precision"/> digits long.
    /// The <c>Sign</c> component is zero exactly when the mantissa is "0".
    /// </remarks>
    /// <param name="precision">Number of digits (&gt; 0) included to the right of decimal point.</param>
    /// <param name="radix">Base ( &gt;= 2, &lt;= 16) for the resulting representation.</param>
    /// <param name="minPrecision">Precision used to distinguish number from zero. Expressed as a power of minPrecision.</param>
    public StringFloatRep ToStringFloatRep(int precision, int radix, int minPrecision)
    {
        if (precision <= 0)
            throw new ArithmeticException("Bad precision argument");

        double log2Radix = Math.Log(radix) / Math.Log(2);
        BigInteger bigRadix = new BigInteger(radix);
        long longMsdPrecision = (long)(log2Radix * minPrecision);
        if (longMsdPrecision is > int.MaxValue or < int.MinValue)
            throw new PrecisionOverflowException();
        int msdPrecision = (int)longMsdPrecision;
        CheckPrecision(msdPrecision);
        int msd = RefineMsd(msdPrecision - 2);

        if (msd == int.MinValue)
            return new StringFloatRep(0, "0", radix, 0);

        int exponent = (int)Math.Ceiling(msd / log2Radix); // Guess for the exponent.  Try to get it usually right.
        int scaleExp = exponent - precision;
        ConstructiveReal scaledValue = scaleExp > 0
            ? FromBigInteger(BigInteger.Pow(radix, scaleExp)).Inverse()
            : FromBigInteger(BigInteger.Pow(radix, -scaleExp));

        ConstructiveReal scaledResult = this * scaledValue;
        BigInteger scaledInt = scaledResult.GetApproximation(0);
        int sign = scaledInt.Sign;
        string mantissa = BigInteger.Abs(scaledInt).ToString(radix);

        while (mantissa.Length < precision)
        {
            scaledResult = scaledResult * FromBigInteger(bigRadix);
            exponent -= 1;
            scaledInt = scaledResult.GetApproximation(0);
            sign = scaledInt.Sign;
            mantissa = BigInteger.Abs(scaledInt).ToString(radix);
        }

        if (mantissa.Length > precision)
        {
            exponent += mantissa.Length - precision;
            mantissa = mantissa.Substring(0, precision);
        }

        return new StringFloatRep(sign, mantissa, radix, exponent);
    }

    /// <summary>
    /// Returns a BigInteger which differs by less than one from the constructive real.
    /// </summary>
    public BigInteger BigIntegerValue() => GetApproximation(0);

    /// <summary>
    /// Returns an int which differs by less than one from the constructive real.
    /// </summary>
    public int IntValue() => (int)BigIntegerValue();

    /// <summary>
    /// Returns a byte which differs by less than one from the constructive real.
    /// </summary>
    public byte ByteValue() => (byte)BigIntegerValue();

    /// <summary>
    /// Returns a long which differs by less than one from the constructive real.
    /// </summary>
    public long LongValue() => (long)BigIntegerValue();

    /// <summary>
    /// Returns a double which differs by less than one in the least represented bit from the constructive real.
    /// </summary>
    public double DoubleValue()
    {
        int myMsd = GetMsd(-1080);
        if (myMsd == int.MinValue) return 0.0;

        int neededPrecision = myMsd - 60;
        double scaledInt = (double)GetApproximation(neededPrecision);
        bool mayUnderflow = neededPrecision < -1000;

        long scaledIntRep = BitConverter.DoubleToInt64Bits(scaledInt);
        long expAdj = mayUnderflow ? neededPrecision + 96 : neededPrecision;
        long origExp = (scaledIntRep >> 52) & 0x7FF;

        if (origExp + expAdj >= 0x7FF)
        {
            return scaledInt < 0.0 ? double.NegativeInfinity : double.PositiveInfinity;
        }

        scaledIntRep += expAdj << 52;
        double result = BitConverter.Int64BitsToDouble(scaledIntRep);

        if (mayUnderflow)
        {
            double two48 = 1L << 48;
            return result / two48 / two48;
        }

        return result;
    }

    /// <summary>
    /// Returns a float which differs by less than one in the least represented bit from the constructive real.
    /// </summary>
    /// <remarks>
    /// Note that double-rounding is not a problem here, since we
    /// cannot, and do not, guarantee correct rounding.
    /// </remarks>
    public float FloatValue() => (float)DoubleValue();

    /// <summary>
    /// Add two constructive reals.
    /// </summary>
    public static ConstructiveReal operator +(ConstructiveReal left, ConstructiveReal right) =>
        new AddConstructiveReal(left, right);

    /// <summary>
    /// Shifts left by n bits.
    /// </summary>
    /// <remarks>
    /// Multiply a constructive real by 2**<paramref name="n"/>.
    /// </remarks>
    /// <param name="value">The value that is shifted left by <paramref name="n"/>.</param>
    /// <param name="n">shift count, may be negative</param>
    public static ConstructiveReal operator <<(ConstructiveReal value, int n)
    {
        CheckPrecision(n);
        return new ShiftedConstructiveReal(value, n);
    }

    /// <summary>
    /// Multiplies the constructive real by 2^(-<paramref name="n"/>).
    /// </summary>
    /// <param name="value">The value that is shifted right by <paramref name="n"/>.</param>
    /// <param name="n">shift count, may be negative</param>
    public static ConstructiveReal operator >> (ConstructiveReal value, int n)
    {
        CheckPrecision(n);
        return new ShiftedConstructiveReal(value, -n);
    }

    /// <summary>
    /// Assumes the constructive real is an integer, preventing unnecessary evaluations.
    /// </summary>
    /// <remarks>
    /// Produce a constructive real equivalent to the original, assuming
    /// the original was an integer.  Undefined results if the original
    /// was not an integer.  Prevents evaluation of digits to the right
    /// of the decimal point, and may thus improve performance.
    /// </remarks>
    public ConstructiveReal AssumeInt() => new AssumedIntConstructiveReal(this);

    /// <summary>
    /// Negates the constructive real number.
    /// </summary>
    public static ConstructiveReal operator -(ConstructiveReal value) => new NegateConstructiveReal(value);

    /// <summary>
    /// The difference between two constructive reals
    /// </summary>
    public static ConstructiveReal operator -(ConstructiveReal left, ConstructiveReal right) =>
        new AddConstructiveReal(left, -right);

    /// <summary>
    /// Multiplies two constructive real numbers.
    /// </summary>
    public static ConstructiveReal operator *(ConstructiveReal left, ConstructiveReal right) =>
        new MultiplyConstructiveReal(left, right);

    /// <summary>
    /// Returns the multiplicative inverse of a constructive real number.
    /// </summary>
    /// <remarks>
    /// <c>x.Inverse()</c> is equivalent to <c>ConstructiveReal.FromInt(1).Divide(x)</c>.
    /// </remarks>
    public ConstructiveReal Inverse() => new InverseConstructiveReal(this);

    /// <summary>
    /// The quotient of two constructive reals.
    /// </summary>
    public static ConstructiveReal operator /(ConstructiveReal left, ConstructiveReal right) =>
        new MultiplyConstructiveReal(left, right.Inverse());

    /// <summary>
    /// Selects one of two constructive reals based on the sign of this value.
    /// </summary>
    /// <remarks>
    /// The real number <paramref name="x"/> if <c>this</c> &lt; 0, or <paramref name="y"/> otherwise.
    /// Requires <paramref name="x"/> = <paramref name="y"/> if <c>this</c> = 0.
    /// Since comparisons may diverge, this is often
    /// a useful alternative to conditionals.
    /// </remarks>
    public ConstructiveReal Select(ConstructiveReal x, ConstructiveReal y) => new SelectConstructiveReal(this, x, y);

    /// <summary>
    /// Returns the maximum of this and another constructive real.
    /// </summary>
    public ConstructiveReal Max(ConstructiveReal x) => (this - x).Select(x, this);

    /// <summary>
    /// Returns the minimum of this and another constructive real.
    /// </summary>
    public ConstructiveReal Min(ConstructiveReal x) => (this - x).Select(this, x);

    /// <summary>
    /// Returns the absolute value of the constructive real.
    /// </summary>
    /// <remarks>
    /// Note that this cannot be written as a conditional.
    /// </remarks>
    public ConstructiveReal Abs() => Select(-this, this);

    /// <summary>
    /// Computes the exponential function e^this.
    /// </summary>
    public ConstructiveReal Exp()
    {
        const int lowPrecision = -10;
        BigInteger roughApprox = GetApproximation(lowPrecision);
        // Handle negative arguments directly;
        // negating and computing inverse can be very expensive.
        if (roughApprox.CompareTo(Big2) > 0 || roughApprox.CompareTo(BigM2) < 0)
        {
            ConstructiveReal squareRoot = (this >> 1).Exp();
            return squareRoot * squareRoot;
        }
        else
        {
            return new PrescaledExpConstructiveReal(this);
        }
    }

    /// <summary>
    /// The ratio of a circle's circumference to its diameter.
    /// </summary>
    public static readonly ConstructiveReal PI = new GaussLegendrePiConstructiveReal();

    /// <summary>
    /// Our old PI implementation. pi/4 = 4*atan(1/5) - atan(1/239)
    /// </summary>
    /// <remarks>
    /// Our old PI implementation. Keep this around for now to allow checking.
    /// This implementation may also be faster for BigInteger implementations
    /// that support only quadratic multiplication, but exhibit high performance
    /// for small computations.  (The standard Android 6 implementation supports
    /// sub-quadratic multiplication, but has high constant overhead.) Many other
    /// atan-based formulas are possible, but based on superficial
    /// experimentation, this is roughly as good as the more complex formulas.
    /// </remarks>
    public static readonly ConstructiveReal AtanPI =
        Four * (Four * AtanReciprocal(5) - AtanReciprocal(239));

    /// <summary>
    /// pi/2
    /// </summary>
    public static readonly ConstructiveReal HalfPI = PI >> 1;

    /// <summary>
    /// Computes the trigonometric cosine function.
    /// </summary>
    public ConstructiveReal Cos()
    {
        BigInteger halfPiMultiples = (this / PI).GetApproximation(-1);
        BigInteger absHalfPiMultiples = BigInteger.Abs(halfPiMultiples);

        if (absHalfPiMultiples.CompareTo(Big2) >= 0)
        {
            BigInteger piMultiples = Scale(halfPiMultiples, -1);
            ConstructiveReal adjustment = PI * FromBigInteger(piMultiples);
            return piMultiples.IsEven ? (this - adjustment).Cos() : -(this - adjustment).Cos();
        }
        else if (BigInteger.Abs(GetApproximation(-1)).CompareTo(Big2) >= 0)
        {
            ConstructiveReal cosHalf = (this >> 1).Cos();
            return ((cosHalf * cosHalf) << 1) - One;
        }
        else
        {
            return new PrescaledCosConstructiveReal(this);
        }
    }

    /// <summary>
    /// The trigonometric sine function.
    /// </summary>
    public ConstructiveReal Sin() => (HalfPI - this).Cos();

    /// <summary>
    /// Computes the trigonometric arc sine function.
    /// </summary>
    public ConstructiveReal Asin()
    {
        BigInteger roughApprox = GetApproximation(-10);
        if (roughApprox.CompareTo(Big750) > 0) // 1/sqrt(2) + a bit
        {
            ConstructiveReal newArg = (One - this * this).Sqrt();
            return newArg.Acos();
        }
        else if (roughApprox.CompareTo(BigM750) < 0)
        {
            return -(-this).Asin();
        }
        else
        {
            return new PrescaledAsinConstructiveReal(this);
        }
    }

    /// <summary>
    /// Computes the trigonometric arc cosine function.
    /// </summary>
    public ConstructiveReal Acos() => HalfPI - Asin();

    private static readonly BigInteger LowLnLimit = Big8; // sixteenths, i.e. 1/2
    private static readonly BigInteger HighLnLimit = new BigInteger(24); // 1.5 * 16
    private static readonly BigInteger Scaled4 = new BigInteger(64); // 4 * 16

    /// <summary>
    /// Computes the natural logarithm (base e).
    /// </summary>
    public ConstructiveReal Ln()
    {
        const int lowPrecision = -4;
        BigInteger roughApprox = GetApproximation(lowPrecision);

        if (roughApprox.CompareTo(Big0) < 0)
        {
            throw new ArithmeticException("ln(negative)");
        }

        if (roughApprox.CompareTo(LowLnLimit) <= 0)
        {
            return -Inverse().Ln();
        }

        if (roughApprox.CompareTo(HighLnLimit) >= 0)
        {
            if (roughApprox.CompareTo(Scaled4) <= 0)
            {
                ConstructiveReal quarter = Sqrt().Sqrt().Ln();
                return quarter << 2;
            }
            else
            {
                int extraBits = (int)roughApprox.GetBitLength() - 3;
                ConstructiveReal scaledResult = (this >> extraBits).Ln();
                return scaledResult + FromInt(extraBits) * Ln2;
            }
        }

        return SimpleLn();
    }

    /// <summary>
    /// The square root of a constructive real.
    /// </summary>
    public ConstructiveReal Sqrt() => new SqrtConstructiveReal(this);
}