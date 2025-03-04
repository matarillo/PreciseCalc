### [PreciseCalc](PreciseCalc.md 'PreciseCalc')

## StringFloatRep Class

A scientific notation representation of an approximation to a constructive real.  
Generated by CR.ToStringFloatRep.

```csharp
public sealed class StringFloatRep
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; StringFloatRep

| Constructors | |
| :--- | :--- |
| [StringFloatRep(int, string, int, int)](PreciseCalc.StringFloatRep.StringFloatRep(int,string,int,int).md 'PreciseCalc.StringFloatRep.StringFloatRep(int, string, int, int)') | Initializes a new instance of the [StringFloatRep](PreciseCalc.StringFloatRep.md 'PreciseCalc.StringFloatRep') class. |

| Properties | |
| :--- | :--- |
| [Exponent](PreciseCalc.StringFloatRep.Exponent.md 'PreciseCalc.StringFloatRep.Exponent') | The mantissa is scaled by radix^exponent. |
| [Mantissa](PreciseCalc.StringFloatRep.Mantissa.md 'PreciseCalc.StringFloatRep.Mantissa') | A string representation of the mantissa. The decimal point is implicitly<br/>to the left of the string of digits, and is not explicitly represented. |
| [Radix](PreciseCalc.StringFloatRep.Radix.md 'PreciseCalc.StringFloatRep.Radix') | The radix of the representation. Also the base of the exponent field. |
| [Sign](PreciseCalc.StringFloatRep.Sign.md 'PreciseCalc.StringFloatRep.Sign') | The sign associated with this approximation. May be -1, 1, or 0. |

| Methods | |
| :--- | :--- |
| [ToString()](PreciseCalc.StringFloatRep.ToString().md 'PreciseCalc.StringFloatRep.ToString()') | Produce a textual representation including the sign and exponent. |
