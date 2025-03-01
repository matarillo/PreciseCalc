### [PreciseCalc](PreciseCalc.md 'PreciseCalc')

## UnifiedReal Class

Computable real numbers, represented so that we can get exact decidable comparisons for a number  
of interesting special cases, including rational computations.  
A real number is represented as the product of two numbers with different representations:  
(A) A BoundedRational that can only represent a subset of the rationals, but supports exact  
computable comparisons. (B) A lazily evaluated "constructive real number" that provides  
operations to evaluate itself to any requested number of digits. Whenever possible, we choose  
(B) to be such that we can describe its exact symbolic value using a CRProperty.

```csharp
public class UnifiedReal
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; UnifiedReal

### Remarks
UnifiedReals, as well as their three components (the rational part, the constructive real part,  
and the property describing the constructive real part) are logically immutable. (The ConstructiveReal  
component physically contains an evaluation cache, which is transparently mutated.)  
Arithmetic operations and operations that produce finite approximations may throw unchecked  
exceptions produced by the underlying ConstructiveReal and BoundedRational packages, including  
PrecisionOverflowException and AbortedException.

| Constructors | |
| :--- | :--- |
| [UnifiedReal(long)](PreciseCalc.UnifiedReal.UnifiedReal(long).md 'PreciseCalc.UnifiedReal.UnifiedReal(long)') | Construct a UnifiedReal from a long. |
| [UnifiedReal(BoundedRational)](PreciseCalc.UnifiedReal.UnifiedReal(PreciseCalc.BoundedRational).md 'PreciseCalc.UnifiedReal.UnifiedReal(PreciseCalc.BoundedRational)') | Construct a UnifiedReal from a BoundedRational. |
| [UnifiedReal(ConstructiveReal)](PreciseCalc.UnifiedReal.UnifiedReal(PreciseCalc.ConstructiveReal).md 'PreciseCalc.UnifiedReal.UnifiedReal(PreciseCalc.ConstructiveReal)') | Construct a UnifiedReal from a ConstructiveReal. |
| [UnifiedReal(BigInteger)](PreciseCalc.UnifiedReal.UnifiedReal(System.Numerics.BigInteger).md 'PreciseCalc.UnifiedReal.UnifiedReal(System.Numerics.BigInteger)') | Construct a UnifiedReal from a BigInteger. |

| Fields | |
| :--- | :--- |
| [E](PreciseCalc.UnifiedReal.E.md 'PreciseCalc.UnifiedReal.E') | The number e. |
| [Half](PreciseCalc.UnifiedReal.Half.md 'PreciseCalc.UnifiedReal.Half') | The number 1/2. |
| [Ln10](PreciseCalc.UnifiedReal.Ln10.md 'PreciseCalc.UnifiedReal.Ln10') | The square root of 2. |
| [MinusHalf](PreciseCalc.UnifiedReal.MinusHalf.md 'PreciseCalc.UnifiedReal.MinusHalf') | The number -1/2. |
| [MinusOne](PreciseCalc.UnifiedReal.MinusOne.md 'PreciseCalc.UnifiedReal.MinusOne') | The number -1. |
| [One](PreciseCalc.UnifiedReal.One.md 'PreciseCalc.UnifiedReal.One') | The number 1. |
| [PI](PreciseCalc.UnifiedReal.PI.md 'PreciseCalc.UnifiedReal.PI') | The number pi. |
| [RadiansPerDegree](PreciseCalc.UnifiedReal.RadiansPerDegree.md 'PreciseCalc.UnifiedReal.RadiansPerDegree') | The number pi/180. |
| [Ten](PreciseCalc.UnifiedReal.Ten.md 'PreciseCalc.UnifiedReal.Ten') | The number 10. |
| [Two](PreciseCalc.UnifiedReal.Two.md 'PreciseCalc.UnifiedReal.Two') | The number 2. |
| [Zero](PreciseCalc.UnifiedReal.Zero.md 'PreciseCalc.UnifiedReal.Zero') | The number 0. |

| Methods | |
| :--- | :--- |
| [Abs()](PreciseCalc.UnifiedReal.Abs().md 'PreciseCalc.UnifiedReal.Abs()') | Absolute value. |
| [Acos()](PreciseCalc.UnifiedReal.Acos().md 'PreciseCalc.UnifiedReal.Acos()') | Return the arccosine of this number. |
| [ApproxEquals(UnifiedReal, int)](PreciseCalc.UnifiedReal.ApproxEquals(PreciseCalc.UnifiedReal,int).md 'PreciseCalc.UnifiedReal.ApproxEquals(PreciseCalc.UnifiedReal, int)') | Equality comparison. May erroneously return true if values differ by less than 2^a, and<br/>!isComparable(u). |
| [ApproxSign(int)](PreciseCalc.UnifiedReal.ApproxSign(int).md 'PreciseCalc.UnifiedReal.ApproxSign(int)') | Return CompareTo(Zero, a). |
| [ApproxWholeNumberBitsGreaterThan(int)](PreciseCalc.UnifiedReal.ApproxWholeNumberBitsGreaterThan(int).md 'PreciseCalc.UnifiedReal.ApproxWholeNumberBitsGreaterThan(int)') | Is the number of bits to the left of the decimal point greater than bound? The result is<br/>inexact: We roughly approximate the whole number bits. bound is non-negative. |
| [Asin()](PreciseCalc.UnifiedReal.Asin().md 'PreciseCalc.UnifiedReal.Asin()') | Return the arcsine of this number. |
| [AsinHalves(int)](PreciseCalc.UnifiedReal.AsinHalves(int).md 'PreciseCalc.UnifiedReal.AsinHalves(int)') | Return asin(n/2). n is between -2 and 2. |
| [Atan()](PreciseCalc.UnifiedReal.Atan().md 'PreciseCalc.UnifiedReal.Atan()') | Return the arctangent of this number. |
| [CommonPower(BoundedRational, BoundedRational)](PreciseCalc.UnifiedReal.CommonPower(PreciseCalc.BoundedRational,PreciseCalc.BoundedRational).md 'PreciseCalc.UnifiedReal.CommonPower(PreciseCalc.BoundedRational, PreciseCalc.BoundedRational)') | Do a and b have a common power? Considers negative exponents. Both a and b must be positive. |
| [CompareTo(UnifiedReal, int)](PreciseCalc.UnifiedReal.CompareTo(PreciseCalc.UnifiedReal,int).md 'PreciseCalc.UnifiedReal.CompareTo(PreciseCalc.UnifiedReal, int)') | Return +1 if this is greater than r, -1 if this is less than r, 0 if the two are equal, or<br/>possibly 0 if the two are within 2^a of each other, and not comparable. |
| [CompareTo(UnifiedReal)](PreciseCalc.UnifiedReal.CompareTo(PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.CompareTo(PreciseCalc.UnifiedReal)') | Return +1 if this is greater than r, -1 if this is less than r, or 0 of the two are known to be<br/>equal. May diverge if the two are equal and !isComparable(r). |
| [Cos()](PreciseCalc.UnifiedReal.Cos().md 'PreciseCalc.UnifiedReal.Cos()') | Return the cosine of this number. |
| [DefinitelyAlgebraic()](PreciseCalc.UnifiedReal.DefinitelyAlgebraic().md 'PreciseCalc.UnifiedReal.DefinitelyAlgebraic()') | Is this number known to be algebraic? |
| [DefinitelyEquals(UnifiedReal)](PreciseCalc.UnifiedReal.DefinitelyEquals(PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.DefinitelyEquals(PreciseCalc.UnifiedReal)') | Returns true if values are definitely known to be equal, false in all other cases. This does<br/>not satisfy the contract for Object.equals(). |
| [DefinitelyIndependent(UnifiedReal)](PreciseCalc.UnifiedReal.DefinitelyIndependent(PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.DefinitelyIndependent(PreciseCalc.UnifiedReal)') | Do we know that this.crFactor is an irrational nonzero multiple of u.crFactor? If this returns<br/>true, then a comparison of the two UnifiedReals cannot diverge, though we don't know of a good<br/>runtime bound. Note that if both values are really tiny, it still may be completely impractical<br/>to compare them. |
| [DefinitelyIrrational()](PreciseCalc.UnifiedReal.DefinitelyIrrational().md 'PreciseCalc.UnifiedReal.DefinitelyIrrational()') | Is this number known to be irrational? |
| [DefinitelyNonzero()](PreciseCalc.UnifiedReal.DefinitelyNonzero().md 'PreciseCalc.UnifiedReal.DefinitelyNonzero()') | Can this number be determined to be definitely nonzero without performing approximate evaluation? |
| [DefinitelyNotEquals(UnifiedReal)](PreciseCalc.UnifiedReal.DefinitelyNotEquals(PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.DefinitelyNotEquals(PreciseCalc.UnifiedReal)') | Returns true if values are definitely known not to be equal based on internal symbolic<br/>information, false in all other cases. Performs no approximate evaluation. |
| [DefinitelyOne()](PreciseCalc.UnifiedReal.DefinitelyOne().md 'PreciseCalc.UnifiedReal.DefinitelyOne()') | Can this number be determined to be definitely one without performing approximate evaluation? |
| [DefinitelyRational()](PreciseCalc.UnifiedReal.DefinitelyRational().md 'PreciseCalc.UnifiedReal.DefinitelyRational()') | Is this number known to be rational? |
| [DefinitelySign()](PreciseCalc.UnifiedReal.DefinitelySign().md 'PreciseCalc.UnifiedReal.DefinitelySign()') | Return CompareTo(Zero). May diverge for Zero argument if !isComparable(Zero). |
| [DefinitelyTranscendental()](PreciseCalc.UnifiedReal.DefinitelyTranscendental().md 'PreciseCalc.UnifiedReal.DefinitelyTranscendental()') | Is this number known to be transcendental? |
| [DefinitelyZero()](PreciseCalc.UnifiedReal.DefinitelyZero().md 'PreciseCalc.UnifiedReal.DefinitelyZero()') | Can this number be determined to be definitely zero without performing approximate<br/>evaluation? |
| [DigitsRequired()](PreciseCalc.UnifiedReal.DigitsRequired().md 'PreciseCalc.UnifiedReal.DigitsRequired()') | Return the number of decimal digits to the right of the decimal point required to represent<br/>the argument exactly. Return Integer.MAX_VALUE if that's not possible. Never returns a value<br/>less than zero, even if r is a power of ten. |
| [Divide(UnifiedReal)](PreciseCalc.UnifiedReal.Divide(PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.Divide(PreciseCalc.UnifiedReal)') | Return x/y |
| [Equals(object)](PreciseCalc.UnifiedReal.Equals(object).md 'PreciseCalc.UnifiedReal.Equals(object)') | UnifiedReals don't have equality or hash codes. |
| [ExactlyDisplayable()](PreciseCalc.UnifiedReal.ExactlyDisplayable().md 'PreciseCalc.UnifiedReal.ExactlyDisplayable()') | Will ToDisplayString() produce an exact representation? |
| [ExactlyTruncatable()](PreciseCalc.UnifiedReal.ExactlyTruncatable().md 'PreciseCalc.UnifiedReal.ExactlyTruncatable()') | Can we compute correctly truncated approximations of this number? |
| [Exp()](PreciseCalc.UnifiedReal.Exp().md 'PreciseCalc.UnifiedReal.Exp()') | Computes the exponential function e^this. |
| [Fact()](PreciseCalc.UnifiedReal.Fact().md 'PreciseCalc.UnifiedReal.Fact()') | Factorial function. Fails if argument is clearly not an integer. May round to nearest integer<br/>if value is close. |
| [FromDouble(double)](PreciseCalc.UnifiedReal.FromDouble(double).md 'PreciseCalc.UnifiedReal.FromDouble(double)') | Construct a UnifiedReal from a double. |
| [FromLong(long)](PreciseCalc.UnifiedReal.FromLong(long).md 'PreciseCalc.UnifiedReal.FromLong(long)') | Construct a UnifiedReal from a long. |
| [GetHashCode()](PreciseCalc.UnifiedReal.GetHashCode().md 'PreciseCalc.UnifiedReal.GetHashCode()') | UnifiedReals don't have equality or hash codes. |
| [Inverse()](PreciseCalc.UnifiedReal.Inverse().md 'PreciseCalc.UnifiedReal.Inverse()') | Return the reciprocal. |
| [IsComparable(UnifiedReal)](PreciseCalc.UnifiedReal.IsComparable(PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.IsComparable(PreciseCalc.UnifiedReal)') | Are this and r exactly comparable? |
| [LeadingBinaryZeroes()](PreciseCalc.UnifiedReal.LeadingBinaryZeroes().md 'PreciseCalc.UnifiedReal.LeadingBinaryZeroes()') | Return an upper bound on the number of leading zero bits. These are the number of 0 bits to the<br/>right of the binary point and to the left of the most significant digit. Return<br/>Integer.MAX_VALUE if we cannot bound it based only on the rational factor and property. |
| [Ln()](PreciseCalc.UnifiedReal.Ln().md 'PreciseCalc.UnifiedReal.Ln()') | Natural logarithm |
| [Log()](PreciseCalc.UnifiedReal.Log().md 'PreciseCalc.UnifiedReal.Log()') | Base 10 logarithm |
| [Multiply(UnifiedReal)](PreciseCalc.UnifiedReal.Multiply(PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.Multiply(PreciseCalc.UnifiedReal)') | Return x*y |
| [Negate()](PreciseCalc.UnifiedReal.Negate().md 'PreciseCalc.UnifiedReal.Negate()') | Return -x |
| [Pow(UnifiedReal)](PreciseCalc.UnifiedReal.Pow(PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.Pow(PreciseCalc.UnifiedReal)') | Return this ^ exp. This is really only well-defined for a positive base, particularly since<br/>0^x is not continuous at zero. 0^0 = 1 (as is epsilon^0), but 0^epsilon is 0. We nonetheless<br/>try to do reasonable things at zero, when we recognize that case. |
| [PropertyCorrect(int)](PreciseCalc.UnifiedReal.PropertyCorrect(int).md 'PreciseCalc.UnifiedReal.PropertyCorrect(int)') | Check that if crProperty uniquely defines a constructive real, then crProperty<br/>and crFactor both describe approximately the same number. |
| [Sin()](PreciseCalc.UnifiedReal.Sin().md 'PreciseCalc.UnifiedReal.Sin()') | Return the sine of this number. |
| [Sqrt()](PreciseCalc.UnifiedReal.Sqrt().md 'PreciseCalc.UnifiedReal.Sqrt()') | Return the square root. This may return a value with a null property, rather than a known<br/>rational, even when the result is rational. |
| [Subtract(UnifiedReal)](PreciseCalc.UnifiedReal.Subtract(PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.Subtract(PreciseCalc.UnifiedReal)') | Return x-y |
| [Tan()](PreciseCalc.UnifiedReal.Tan().md 'PreciseCalc.UnifiedReal.Tan()') | Return the tangent of this number. |
| [ToBigInteger()](PreciseCalc.UnifiedReal.ToBigInteger().md 'PreciseCalc.UnifiedReal.ToBigInteger()') | Returns equivalent BigInteger result if it exists, null if not. |
| [ToBoundedRational()](PreciseCalc.UnifiedReal.ToBoundedRational().md 'PreciseCalc.UnifiedReal.ToBoundedRational()') | Return equivalent BoundedRational, if known to exist, null otherwise |
| [ToConstructiveReal()](PreciseCalc.UnifiedReal.ToConstructiveReal().md 'PreciseCalc.UnifiedReal.ToConstructiveReal()') | Convert to a ConstructiveReal representation |
| [ToDisplayString()](PreciseCalc.UnifiedReal.ToDisplayString().md 'PreciseCalc.UnifiedReal.ToDisplayString()') | Convert to a readable string using radian version of trig functions.<br/>Use improper fractions, and no sub-and super-scripts. |
| [ToDisplayString(bool, bool, bool)](PreciseCalc.UnifiedReal.ToDisplayString(bool,bool,bool).md 'PreciseCalc.UnifiedReal.ToDisplayString(bool, bool, bool)') | Convert to readable String. Intended for user output. Produces exact expression when possible.<br/>If degrees is true, then any trig functions in the output will refer to the degree versions.<br/>Otherwise, the radian versions are used. |
| [ToDouble()](PreciseCalc.UnifiedReal.ToDouble().md 'PreciseCalc.UnifiedReal.ToDouble()') | Return a double approximation. Rational arguments are currently rounded to nearest, with ties<br/>away from zero. TODO: Improve rounding. |
| [ToString()](PreciseCalc.UnifiedReal.ToString().md 'PreciseCalc.UnifiedReal.ToString()') | Convert to String reflecting raw representation. Debug or log messages only, not pretty. |
| [ToStringTruncated(int)](PreciseCalc.UnifiedReal.ToStringTruncated(int).md 'PreciseCalc.UnifiedReal.ToStringTruncated(int)') | Returns a truncated representation of the result.<br/>If [ExactlyTruncatable()](PreciseCalc.UnifiedReal.ExactlyTruncatable().md 'PreciseCalc.UnifiedReal.ExactlyTruncatable()'), we round correctly towards zero. Otherwise, the resulting digit<br/>string may occasionally be rounded up instead.<br/>Always includes a decimal point in the result.<br/>The result includes n digits to the right of the decimal point. |

| Operators | |
| :--- | :--- |
| [operator +(UnifiedReal, UnifiedReal)](PreciseCalc.UnifiedReal.op_Addition(PreciseCalc.UnifiedReal,PreciseCalc.UnifiedReal).md 'PreciseCalc.UnifiedReal.op_Addition(PreciseCalc.UnifiedReal, PreciseCalc.UnifiedReal)') | Return x+y |
