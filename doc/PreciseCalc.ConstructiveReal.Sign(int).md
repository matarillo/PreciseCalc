### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.Sign(int) Method

Determines the sign of the value at a given precision.

```csharp
public int Sign(int precision);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.Sign(int).precision'></a>

`precision` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

### Remarks
Equivalent to `CompareTo(ConstructiveReal.FromInt(0), precision)`