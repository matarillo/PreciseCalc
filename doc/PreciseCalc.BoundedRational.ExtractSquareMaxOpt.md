### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')

## BoundedRational.ExtractSquareMaxOpt Field

Max integer for which [PreciseCalc.BoundedRational.ExtractSquare(System.Numerics.BigInteger)](https://docs.microsoft.com/en-us/dotnet/api/PreciseCalc.BoundedRational.ExtractSquare#PreciseCalc_BoundedRational_ExtractSquare_System_Numerics_BigInteger_ 'PreciseCalc.BoundedRational.ExtractSquare(System.Numerics.BigInteger)') is guaranteed to be optimal.

```csharp
public const int ExtractSquareMaxOpt = 43;
```

#### Field Value
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

### Remarks
We currently fail to do so for 44 = 11*4, but succeed for all perfect squares*n, with n <= 10  
and numerator and denominator size < [PreciseCalc.BoundedRational.ExtractSquareMaxLen](https://docs.microsoft.com/en-us/dotnet/api/PreciseCalc.BoundedRational.ExtractSquareMaxLen 'PreciseCalc.BoundedRational.ExtractSquareMaxLen').