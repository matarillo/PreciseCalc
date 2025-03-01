### [PreciseCalc](PreciseCalc.md 'PreciseCalc')

## ConstructiveReal Class

Represents a constructive real number, allowing arbitrary-precision calculations.

```csharp
public abstract class ConstructiveReal
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; ConstructiveReal

| Fields | |
| :--- | :--- |
| [AtanPI](PreciseCalc.ConstructiveReal.AtanPI.md 'PreciseCalc.ConstructiveReal.AtanPI') | Our old PI implementation. pi/4 = 4*atan(1/5) - atan(1/239) |
| [HalfPI](PreciseCalc.ConstructiveReal.HalfPI.md 'PreciseCalc.ConstructiveReal.HalfPI') | pi/2 |
| [One](PreciseCalc.ConstructiveReal.One.md 'PreciseCalc.ConstructiveReal.One') | Predefined constant for the value 1. |
| [PI](PreciseCalc.ConstructiveReal.PI.md 'PreciseCalc.ConstructiveReal.PI') | The ratio of a circle's circumference to its diameter. |
| [PleaseStop](PreciseCalc.ConstructiveReal.PleaseStop.md 'PreciseCalc.ConstructiveReal.PleaseStop') | Setting this to true requests that all computations be aborted by<br/>throwing AbortedException.<br/>Must be rest to false before any further<br/>computation. |
| [Zero](PreciseCalc.ConstructiveReal.Zero.md 'PreciseCalc.ConstructiveReal.Zero') | Predefined constant for the value 0. |

| Methods | |
| :--- | :--- |
| [Abs()](PreciseCalc.ConstructiveReal.Abs().md 'PreciseCalc.ConstructiveReal.Abs()') | Returns the absolute value of the constructive real. |
| [Acos()](PreciseCalc.ConstructiveReal.Acos().md 'PreciseCalc.ConstructiveReal.Acos()') | Computes the trigonometric arc cosine function. |
| [Asin()](PreciseCalc.ConstructiveReal.Asin().md 'PreciseCalc.ConstructiveReal.Asin()') | Computes the trigonometric arc sine function. |
| [AssumeInt()](PreciseCalc.ConstructiveReal.AssumeInt().md 'PreciseCalc.ConstructiveReal.AssumeInt()') | Assumes the constructive real is an integer, preventing unnecessary evaluations. |
| [BigIntegerValue()](PreciseCalc.ConstructiveReal.BigIntegerValue().md 'PreciseCalc.ConstructiveReal.BigIntegerValue()') | Returns a BigInteger which differs by less than one from the constructive real. |
| [ByteValue()](PreciseCalc.ConstructiveReal.ByteValue().md 'PreciseCalc.ConstructiveReal.ByteValue()') | Returns a byte which differs by less than one from the constructive real. |
| [CompareTo(ConstructiveReal, int, int)](PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int,int).md 'PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal, int, int)') | Compares this value with another constructive real, refining precision iteratively. |
| [CompareTo(ConstructiveReal, int)](PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int).md 'PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal, int)') | Compares this value with another constructive real at a given precision. |
| [CompareTo(ConstructiveReal)](PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal).md 'PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal)') | Compares this value with another constructive real. |
| [Cos()](PreciseCalc.ConstructiveReal.Cos().md 'PreciseCalc.ConstructiveReal.Cos()') | Computes the trigonometric cosine function. |
| [DoubleValue()](PreciseCalc.ConstructiveReal.DoubleValue().md 'PreciseCalc.ConstructiveReal.DoubleValue()') | Returns a double which differs by less than one in the least represented bit from the constructive real. |
| [Exp()](PreciseCalc.ConstructiveReal.Exp().md 'PreciseCalc.ConstructiveReal.Exp()') | Computes the exponential function e^this. |
| [FloatValue()](PreciseCalc.ConstructiveReal.FloatValue().md 'PreciseCalc.ConstructiveReal.FloatValue()') | Returns a float which differs by less than one in the least represented bit from the constructive real. |
| [FromBigInteger(BigInteger)](PreciseCalc.ConstructiveReal.FromBigInteger(System.Numerics.BigInteger).md 'PreciseCalc.ConstructiveReal.FromBigInteger(System.Numerics.BigInteger)') | Creates a constructive real number from a [System.Numerics.BigInteger](https://docs.microsoft.com/en-us/dotnet/api/System.Numerics.BigInteger 'System.Numerics.BigInteger'). |
| [FromDouble(double)](PreciseCalc.ConstructiveReal.FromDouble(double).md 'PreciseCalc.ConstructiveReal.FromDouble(double)') | Creates a constructive real number from a [System.Double](https://docs.microsoft.com/en-us/dotnet/api/System.Double 'System.Double'). |
| [FromFloat(float)](PreciseCalc.ConstructiveReal.FromFloat(float).md 'PreciseCalc.ConstructiveReal.FromFloat(float)') | The constructive real number corresponding to a [System.Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single 'System.Single'). |
| [FromInt(int)](PreciseCalc.ConstructiveReal.FromInt(int).md 'PreciseCalc.ConstructiveReal.FromInt(int)') | Creates a constructive real number from an [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32'). |
| [FromLong(long)](PreciseCalc.ConstructiveReal.FromLong(long).md 'PreciseCalc.ConstructiveReal.FromLong(long)') | Creates a constructive real number from a [System.Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64 'System.Int64'). |
| [FromString(string, int)](PreciseCalc.ConstructiveReal.FromString(string,int).md 'PreciseCalc.ConstructiveReal.FromString(string, int)') | Creates a constructive real number from a string representation in the given radix. |
| [GetApproximation(int)](PreciseCalc.ConstructiveReal.GetApproximation(int).md 'PreciseCalc.ConstructiveReal.GetApproximation(int)') | Returns the approximation of the value scaled by 2^[precision](PreciseCalc.ConstructiveReal.GetApproximation(int).md#PreciseCalc.ConstructiveReal.GetApproximation(int).precision 'PreciseCalc.ConstructiveReal.GetApproximation(int).precision'), rounded to an integer.<br/>The error in the result is strictly < 1. |
| [IntValue()](PreciseCalc.ConstructiveReal.IntValue().md 'PreciseCalc.ConstructiveReal.IntValue()') | Returns an int which differs by less than one from the constructive real. |
| [Inverse()](PreciseCalc.ConstructiveReal.Inverse().md 'PreciseCalc.ConstructiveReal.Inverse()') | Returns the multiplicative inverse of a constructive real number. |
| [Ln()](PreciseCalc.ConstructiveReal.Ln().md 'PreciseCalc.ConstructiveReal.Ln()') | Computes the natural logarithm (base e). |
| [LongValue()](PreciseCalc.ConstructiveReal.LongValue().md 'PreciseCalc.ConstructiveReal.LongValue()') | Returns a long which differs by less than one from the constructive real. |
| [Max(ConstructiveReal)](PreciseCalc.ConstructiveReal.Max(PreciseCalc.ConstructiveReal).md 'PreciseCalc.ConstructiveReal.Max(PreciseCalc.ConstructiveReal)') | Returns the maximum of this and another constructive real. |
| [Min(ConstructiveReal)](PreciseCalc.ConstructiveReal.Min(PreciseCalc.ConstructiveReal).md 'PreciseCalc.ConstructiveReal.Min(PreciseCalc.ConstructiveReal)') | Returns the minimum of this and another constructive real. |
| [Select(ConstructiveReal, ConstructiveReal)](PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md 'PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal)') | Selects one of two constructive reals based on the sign of this value. |
| [Sign()](PreciseCalc.ConstructiveReal.Sign().md 'PreciseCalc.ConstructiveReal.Sign()') | Determines the sign of the value, refining precision iteratively. |
| [Sign(int)](PreciseCalc.ConstructiveReal.Sign(int).md 'PreciseCalc.ConstructiveReal.Sign(int)') | Determines the sign of the value at a given precision. |
| [Sin()](PreciseCalc.ConstructiveReal.Sin().md 'PreciseCalc.ConstructiveReal.Sin()') | The trigonometric sine function. |
| [Sqrt()](PreciseCalc.ConstructiveReal.Sqrt().md 'PreciseCalc.ConstructiveReal.Sqrt()') | The square root of a constructive real. |
| [ToString()](PreciseCalc.ConstructiveReal.ToString().md 'PreciseCalc.ConstructiveReal.ToString()') | Converts the number to a decimal string with default precision. |
| [ToString(int, int)](PreciseCalc.ConstructiveReal.ToString(int,int).md 'PreciseCalc.ConstructiveReal.ToString(int, int)') | Converts the number to a string representation with the given precision and radix. |
| [ToString(int)](PreciseCalc.ConstructiveReal.ToString(int).md 'PreciseCalc.ConstructiveReal.ToString(int)') | Converts the number to a decimal string with the given precision. |
| [ToStringFloatRep(int, int, int)](PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).md 'PreciseCalc.ConstructiveReal.ToStringFloatRep(int, int, int)') | Returns a textual scientific notation representation. |

| Operators | |
| :--- | :--- |
| [operator +(ConstructiveReal, ConstructiveReal)](PreciseCalc.ConstructiveReal.op_Addition(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md 'PreciseCalc.ConstructiveReal.op_Addition(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal)') | Add two constructive reals. |
| [operator /(ConstructiveReal, ConstructiveReal)](PreciseCalc.ConstructiveReal.op_Division(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md 'PreciseCalc.ConstructiveReal.op_Division(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal)') | The quotient of two constructive reals. |
| [operator &lt;&lt;(ConstructiveReal, int)](PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal,int).md 'PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal, int)') | Shifts left by n bits. |
| [operator *(ConstructiveReal, ConstructiveReal)](PreciseCalc.ConstructiveReal.op_Multiply(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md 'PreciseCalc.ConstructiveReal.op_Multiply(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal)') | Multiplies two constructive real numbers. |
| [operator &gt;&gt;(ConstructiveReal, int)](PreciseCalc.ConstructiveReal.op_RightShift(PreciseCalc.ConstructiveReal,int).md 'PreciseCalc.ConstructiveReal.op_RightShift(PreciseCalc.ConstructiveReal, int)') | Multiplies the constructive real by 2^(-[n](PreciseCalc.ConstructiveReal.op_RightShift(PreciseCalc.ConstructiveReal,int).md#PreciseCalc.ConstructiveReal.op_RightShift(PreciseCalc.ConstructiveReal,int).n 'PreciseCalc.ConstructiveReal.op_RightShift(PreciseCalc.ConstructiveReal, int).n')). |
| [operator -(ConstructiveReal, ConstructiveReal)](PreciseCalc.ConstructiveReal.op_Subtraction(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md 'PreciseCalc.ConstructiveReal.op_Subtraction(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal)') | The difference between two constructive reals |
| [operator -(ConstructiveReal)](PreciseCalc.ConstructiveReal.op_UnaryNegation(PreciseCalc.ConstructiveReal).md 'PreciseCalc.ConstructiveReal.op_UnaryNegation(PreciseCalc.ConstructiveReal)') | Negates the constructive real number. |
