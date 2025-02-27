### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.CompareTo(ConstructiveReal) Method

Compares this value with another constructive real.

```csharp
public int CompareTo(PreciseCalc.ConstructiveReal x);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal).x'></a>

`x` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

### Remarks
Return -1 if `this < x`, or +1 if `this > x`.  
Should be called only if `this != x`.  
If `this == x`, this will not terminate correctly; typically it  
will run until it exhausts memory.  
If the two constructive reals may be equal, the two or 3 argument  
version of `CompareTo` should be used.