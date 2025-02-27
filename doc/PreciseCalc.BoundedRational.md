### [PreciseCalc](PreciseCalc.md 'PreciseCalc')

## BoundedRational Struct

Represents rational numbers that may become null if they get too big.

```csharp
public readonly struct BoundedRational :
System.IEquatable<PreciseCalc.BoundedRational>,
System.IComparable<PreciseCalc.BoundedRational>
```

Implements [System.IEquatable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1'), [System.IComparable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.IComparable-1 'System.IComparable`1')[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.IComparable-1 'System.IComparable`1')

### Remarks
  
For many operations, if the length of the numerator plus the length of the denominator  
exceeds a maximum size, we simply return null, and rely on our caller do something else.  
We currently never return null for a pure integer or for a BoundedRational that has just been constructed.  
  
We also implement a number of irrational functions.  
These return a non-null result only when the result is known to be rational.

| Constructors | |
| :--- | :--- |
| [BoundedRational()](PreciseCalc.BoundedRational.BoundedRational().md 'PreciseCalc.BoundedRational.BoundedRational()') | Create an invalid bounded rational. |
| [BoundedRational(long, long)](PreciseCalc.BoundedRational.BoundedRational(long,long).md 'PreciseCalc.BoundedRational.BoundedRational(long, long)') | Create a valid bounded rational. |
| [BoundedRational(long)](PreciseCalc.BoundedRational.BoundedRational(long).md 'PreciseCalc.BoundedRational.BoundedRational(long)') | Create a valid bounded rational. |
| [BoundedRational(BigInteger, BigInteger)](PreciseCalc.BoundedRational.BoundedRational(System.Numerics.BigInteger,System.Numerics.BigInteger).md 'PreciseCalc.BoundedRational.BoundedRational(System.Numerics.BigInteger, System.Numerics.BigInteger)') | Create a valid bounded rational. |
| [BoundedRational(BigInteger)](PreciseCalc.BoundedRational.BoundedRational(System.Numerics.BigInteger).md 'PreciseCalc.BoundedRational.BoundedRational(System.Numerics.BigInteger)') | Create a valid bounded rational. |

| Fields | |
| :--- | :--- |
| [ExtractSquareMaxOpt](PreciseCalc.BoundedRational.ExtractSquareMaxOpt.md 'PreciseCalc.BoundedRational.ExtractSquareMaxOpt') | Max integer for which [PreciseCalc.BoundedRational.ExtractSquare(System.Numerics.BigInteger)](https://docs.microsoft.com/en-us/dotnet/api/PreciseCalc.BoundedRational.ExtractSquare#PreciseCalc_BoundedRational_ExtractSquare_System_Numerics_BigInteger_ 'PreciseCalc.BoundedRational.ExtractSquare(System.Numerics.BigInteger)') is guaranteed to be optimal. |
| [Half](PreciseCalc.BoundedRational.Half.md 'PreciseCalc.BoundedRational.Half') | Singleton for one half. |
| [MinusHalf](PreciseCalc.BoundedRational.MinusHalf.md 'PreciseCalc.BoundedRational.MinusHalf') | Singleton for minus one half. |
| [MinusOne](PreciseCalc.BoundedRational.MinusOne.md 'PreciseCalc.BoundedRational.MinusOne') | Singleton for minus one. |
| [MinusTwo](PreciseCalc.BoundedRational.MinusTwo.md 'PreciseCalc.BoundedRational.MinusTwo') | Singleton for minus two. |
| [Null](PreciseCalc.BoundedRational.Null.md 'PreciseCalc.BoundedRational.Null') | Singleton for an invalid bounded rational. |
| [One](PreciseCalc.BoundedRational.One.md 'PreciseCalc.BoundedRational.One') | Singleton for one. |
| [Quarter](PreciseCalc.BoundedRational.Quarter.md 'PreciseCalc.BoundedRational.Quarter') | Singleton for one fourth. |
| [Sixth](PreciseCalc.BoundedRational.Sixth.md 'PreciseCalc.BoundedRational.Sixth') | Singleton for one sixth. |
| [Ten](PreciseCalc.BoundedRational.Ten.md 'PreciseCalc.BoundedRational.Ten') | Singleton for ten. |
| [Third](PreciseCalc.BoundedRational.Third.md 'PreciseCalc.BoundedRational.Third') | Singleton for one third. |
| [Three](PreciseCalc.BoundedRational.Three.md 'PreciseCalc.BoundedRational.Three') | Singleton for three. |
| [Twelve](PreciseCalc.BoundedRational.Twelve.md 'PreciseCalc.BoundedRational.Twelve') | Singleton for twelve. |
| [Two](PreciseCalc.BoundedRational.Two.md 'PreciseCalc.BoundedRational.Two') | Singleton for two. |
| [Zero](PreciseCalc.BoundedRational.Zero.md 'PreciseCalc.BoundedRational.Zero') | Singleton for zero. |

| Properties | |
| :--- | :--- |
| [BitLength](PreciseCalc.BoundedRational.BitLength.md 'PreciseCalc.BoundedRational.BitLength') | Number of bits in the representation. Makes the most sense for the result of Reduce(),<br/>since it does not implicitly reduce. |
| [Denominator](PreciseCalc.BoundedRational.Denominator.md 'PreciseCalc.BoundedRational.Denominator') | Returns the denominator of the bounded rational. |
| [DigitsRequired](PreciseCalc.BoundedRational.DigitsRequired.md 'PreciseCalc.BoundedRational.DigitsRequired') | Computes the number of decimal digits required for exact representation. |
| [IsNull](PreciseCalc.BoundedRational.IsNull.md 'PreciseCalc.BoundedRational.IsNull') | Returns true if the bounded rational is null (not valid). |
| [NumDen](PreciseCalc.BoundedRational.NumDen.md 'PreciseCalc.BoundedRational.NumDen') | Returns the pair of numerator and denominator. |
| [Numerator](PreciseCalc.BoundedRational.Numerator.md 'PreciseCalc.BoundedRational.Numerator') | Returns the denominator of the bounded rational. |
| [Sign](PreciseCalc.BoundedRational.Sign.md 'PreciseCalc.BoundedRational.Sign') | Returns the sign of this rational number. |
| [WholeNumberBits](PreciseCalc.BoundedRational.WholeNumberBits.md 'PreciseCalc.BoundedRational.WholeNumberBits') | Approximate number of bits to left of binary point.<br/>Negative indicates leading zeroes to the right of binary point. |

| Methods | |
| :--- | :--- |
| [ApproxLog2Abs()](PreciseCalc.BoundedRational.ApproxLog2Abs().md 'PreciseCalc.BoundedRational.ApproxLog2Abs()') | Returns an approximation of the base 2 log of the absolute value.<br/>We assume this is nonzero.<br/>The result is either 0 or within 20% of the truth. |
| [CompareTo(BoundedRational)](PreciseCalc.BoundedRational.CompareTo(PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.CompareTo(PreciseCalc.BoundedRational)') | Compares this bounded rational to another. |
| [CompareToOne()](PreciseCalc.BoundedRational.CompareToOne().md 'PreciseCalc.BoundedRational.CompareToOne()') | Equivalent to CompareTo(BoundedRational.One) but faster. |
| [Equals(object)](PreciseCalc.BoundedRational.Equals(object).md 'PreciseCalc.BoundedRational.Equals(object)') | Compares this bounded rational to another. |
| [Equals(BoundedRational)](PreciseCalc.BoundedRational.Equals(PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.Equals(PreciseCalc.BoundedRational)') | Compares this bounded rational to another. |
| [ExtractSquareReduced()](PreciseCalc.BoundedRational.ExtractSquareReduced().md 'PreciseCalc.BoundedRational.ExtractSquareReduced()') | Returns a pair p such that p[0]^2 * p[1] = this.<br/>Tries to maximize p[0]. This rational is assumed to be in reduced form. |
| [ExtractSquareWillSucceed()](PreciseCalc.BoundedRational.ExtractSquareWillSucceed().md 'PreciseCalc.BoundedRational.ExtractSquareWillSucceed()') | Will ExtractSquareReduced guarantee that p[1] is not a perfect square?<br/>This rational is assumed to be in reduced form. |
| [Floor()](PreciseCalc.BoundedRational.Floor().md 'PreciseCalc.BoundedRational.Floor()') | Returns closest integer no larger than this. |
| [FromDouble(double)](PreciseCalc.BoundedRational.FromDouble(double).md 'PreciseCalc.BoundedRational.FromDouble(double)') | Produce BoundedRational equal to the given double. |
| [FromLong(long)](PreciseCalc.BoundedRational.FromLong(long).md 'PreciseCalc.BoundedRational.FromLong(long)') | Produce BoundedRational equal to the given long. |
| [GetHashCode()](PreciseCalc.BoundedRational.GetHashCode().md 'PreciseCalc.BoundedRational.GetHashCode()') | Returns the hash code of this bounded rational. |
| [Inverse(BoundedRational)](PreciseCalc.BoundedRational.Inverse(PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.Inverse(PreciseCalc.BoundedRational)') | Returns the inverse of a bounded rational. |
| [NthRoot(BoundedRational, int)](PreciseCalc.BoundedRational.NthRoot(PreciseCalc.BoundedRational,int).md 'PreciseCalc.BoundedRational.NthRoot(PreciseCalc.BoundedRational, int)') | Compute nth root exactly. Returns null if irrational. |
| [Pow(BoundedRational, BoundedRational)](PreciseCalc.BoundedRational.Pow(PreciseCalc.BoundedRational,PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.Pow(PreciseCalc.BoundedRational, PreciseCalc.BoundedRational)') | Computes r^exp |
| [Pow(BigInteger)](PreciseCalc.BoundedRational.Pow(System.Numerics.BigInteger).md 'PreciseCalc.BoundedRational.Pow(System.Numerics.BigInteger)') | Compute an integral power of this rational. |
| [Reduce()](PreciseCalc.BoundedRational.Reduce().md 'PreciseCalc.BoundedRational.Reduce()') | Reduces this fraction to the lowest terms. |
| [Sqrt(BoundedRational)](PreciseCalc.BoundedRational.Sqrt(PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.Sqrt(PreciseCalc.BoundedRational)') | Returns the square root if rational, null otherwise. |
| [ToBigInteger()](PreciseCalc.BoundedRational.ToBigInteger().md 'PreciseCalc.BoundedRational.ToBigInteger()') | Returns equivalent BigInteger if it exists, null if not. |
| [ToConstructiveReal()](PreciseCalc.BoundedRational.ToConstructiveReal().md 'PreciseCalc.BoundedRational.ToConstructiveReal()') | Returns the constructive real representation of this rational number. |
| [ToDisplayString(bool, bool)](PreciseCalc.BoundedRational.ToDisplayString(bool,bool).md 'PreciseCalc.BoundedRational.ToDisplayString(bool, bool)') | Converts to a readable string representation. Renamed from toNiceString. |
| [ToDouble()](PreciseCalc.BoundedRational.ToDouble().md 'PreciseCalc.BoundedRational.ToDouble()') | Return a double approximation. |
| [ToInt32()](PreciseCalc.BoundedRational.ToInt32().md 'PreciseCalc.BoundedRational.ToInt32()') | Returns the integer value of this rational number.<br/>Throws an exception if this is not an integer. |
| [ToString()](PreciseCalc.BoundedRational.ToString().md 'PreciseCalc.BoundedRational.ToString()') | Converts to a string representation. |
| [ToStringTruncated(int)](PreciseCalc.BoundedRational.ToStringTruncated(int).md 'PreciseCalc.BoundedRational.ToStringTruncated(int)') | Truncate the rational representation to specified digits. |

| Operators | |
| :--- | :--- |
| [operator +(BoundedRational, BoundedRational)](PreciseCalc.BoundedRational.op_Addition(PreciseCalc.BoundedRational,PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.op_Addition(PreciseCalc.BoundedRational, PreciseCalc.BoundedRational)') | Adds two bounded rationals. |
| [operator /(BoundedRational, BoundedRational)](PreciseCalc.BoundedRational.op_Division(PreciseCalc.BoundedRational,PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.op_Division(PreciseCalc.BoundedRational, PreciseCalc.BoundedRational)') | Divides two bounded rationals. |
| [operator ==(BoundedRational, BoundedRational)](PreciseCalc.BoundedRational.op_Equality(PreciseCalc.BoundedRational,PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.op_Equality(PreciseCalc.BoundedRational, PreciseCalc.BoundedRational)') | Compares two bounded rationals. |
| [operator !=(BoundedRational, BoundedRational)](PreciseCalc.BoundedRational.op_Inequality(PreciseCalc.BoundedRational,PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.op_Inequality(PreciseCalc.BoundedRational, PreciseCalc.BoundedRational)') | Compares two bounded rationals. |
| [operator *(BoundedRational, BoundedRational)](PreciseCalc.BoundedRational.op_Multiply(PreciseCalc.BoundedRational,PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.op_Multiply(PreciseCalc.BoundedRational, PreciseCalc.BoundedRational)') | Multiplies two bounded rationals. |
| [operator -(BoundedRational, BoundedRational)](PreciseCalc.BoundedRational.op_Subtraction(PreciseCalc.BoundedRational,PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.op_Subtraction(PreciseCalc.BoundedRational, PreciseCalc.BoundedRational)') | Subtracts two bounded rationals. |
| [operator -(BoundedRational)](PreciseCalc.BoundedRational.op_UnaryNegation(PreciseCalc.BoundedRational).md 'PreciseCalc.BoundedRational.op_UnaryNegation(PreciseCalc.BoundedRational)') | Negates a bounded rational. |
