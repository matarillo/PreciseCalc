### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.ToStringFloatRep(int, int, int) Method

Returns a textual scientific notation representation.

```csharp
public PreciseCalc.StringFloatRep ToStringFloatRep(int precision, int radix, int minPrecision);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).precision'></a>

`precision` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Number of digits (> 0) included to the right of decimal point.

<a name='PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).radix'></a>

`radix` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Base ( >= 2, <= 16) for the resulting representation.

<a name='PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).minPrecision'></a>

`minPrecision` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Precision used to distinguish number from zero. Expressed as a power of minPrecision.

#### Returns
[StringFloatRep](PreciseCalc.StringFloatRep.md 'PreciseCalc.StringFloatRep')

### Remarks
Return a textual scientific notation representation  
accurate to [precision](PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).md#PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).precision 'PreciseCalc.ConstructiveReal.ToStringFloatRep(int, int, int).precision') places to the right of the decimal point.  
[precision](PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).md#PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).precision 'PreciseCalc.ConstructiveReal.ToStringFloatRep(int, int, int).precision') must be nonnegative.  
A value smaller than [radix](PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).md#PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).radix 'PreciseCalc.ConstructiveReal.ToStringFloatRep(int, int, int).radix') ** -[minPrecision](PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).md#PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).minPrecision 'PreciseCalc.ConstructiveReal.ToStringFloatRep(int, int, int).minPrecision') may be displayed as 0.  
The `Mantissa` component of the result is either "0" or exactly [precision](PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).md#PreciseCalc.ConstructiveReal.ToStringFloatRep(int,int,int).precision 'PreciseCalc.ConstructiveReal.ToStringFloatRep(int, int, int).precision') digits long.  
The `Sign` component is zero exactly when the mantissa is "0".