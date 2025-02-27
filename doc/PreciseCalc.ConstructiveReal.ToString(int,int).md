### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.ToString(int, int) Method

Converts the number to a string representation with the given precision and radix.

```csharp
public string ToString(int n, int radix);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.ToString(int,int).n'></a>

`n` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Number of digits (>= 0) included to the right of decimal point

<a name='PreciseCalc.ConstructiveReal.ToString(int,int).radix'></a>

`radix` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Base ( >= 2, <= 16) for the resulting representation.

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

### Remarks
Return a textual representation accurate to [n](PreciseCalc.ConstructiveReal.ToString(int,int).md#PreciseCalc.ConstructiveReal.ToString(int,int).n 'PreciseCalc.ConstructiveReal.ToString(int, int).n')  
places to the right of the decimal point.  
[n](PreciseCalc.ConstructiveReal.ToString(int,int).md#PreciseCalc.ConstructiveReal.ToString(int,int).n 'PreciseCalc.ConstructiveReal.ToString(int, int).n') must be nonnegative.