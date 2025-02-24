### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.FloatValue() Method

Returns a float which differs by less than one in the least represented bit from the constructive real.

```csharp
public float FloatValue();
```

#### Returns
[System.Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single 'System.Single')

### Remarks
Note that double-rounding is not a problem here, since we  
cannot, and do not, guarantee correct rounding.