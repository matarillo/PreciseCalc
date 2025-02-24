### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.CompareTo(ConstructiveReal, int) Method

Compares this value with another constructive real at a given precision.

```csharp
public int CompareTo(PreciseCalc.ConstructiveReal x, int absPrecision);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int).x'></a>

`x` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

<a name='PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int).absPrecision'></a>

`absPrecision` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

### Remarks
Approximate comparison with only an absolute tolerance.  
Identical to the three argument version, but without a relative  
tolerance.  
Result is 0 if both constructive reals are equal, indeterminate  
if they differ by less than 2**[absPrecision](PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int).md#PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int).absPrecision 'PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal, int).absPrecision') .