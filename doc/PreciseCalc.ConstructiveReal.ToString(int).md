### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.ToString(int) Method

Converts the number to a decimal string with the given precision.

```csharp
public string ToString(int n);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.ToString(int).n'></a>

`n` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Number of digits included to the right of decimal point

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

### Remarks
Equivalent to `ToString(n, 10)`