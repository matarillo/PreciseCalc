### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.GetApproximation(int) Method

Returns the approximation of the value scaled by 2^[precision](PreciseCalc.ConstructiveReal.GetApproximation(int).md#PreciseCalc.ConstructiveReal.GetApproximation(int).precision 'PreciseCalc.ConstructiveReal.GetApproximation(int).precision'), rounded to an integer.  
The error in the result is strictly < 1.

```csharp
public virtual System.Numerics.BigInteger GetApproximation(int precision);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.GetApproximation(int).precision'></a>

`precision` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

#### Returns
[System.Numerics.BigInteger](https://docs.microsoft.com/en-us/dotnet/api/System.Numerics.BigInteger 'System.Numerics.BigInteger')

### Remarks
Produces the same answer as [PreciseCalc.ConstructiveReal.Approximate(System.Int32)](https://docs.microsoft.com/en-us/dotnet/api/PreciseCalc.ConstructiveReal.Approximate#PreciseCalc_ConstructiveReal_Approximate_System_Int32_ 'PreciseCalc.ConstructiveReal.Approximate(System.Int32)'), but uses and  
maintains a cached approximation.  
Normally not overridden, and called only from [PreciseCalc.ConstructiveReal.Approximate(System.Int32)](https://docs.microsoft.com/en-us/dotnet/api/PreciseCalc.ConstructiveReal.Approximate#PreciseCalc_ConstructiveReal_Approximate_System_Int32_ 'PreciseCalc.ConstructiveReal.Approximate(System.Int32)')  
methods in subclasses.  Not needed if the provided operations  
on constructive reals suffice.