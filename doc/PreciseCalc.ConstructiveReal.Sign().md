### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.Sign() Method

Determines the sign of the value, refining precision iteratively.

```csharp
public int Sign();
```

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

### Remarks
Return `-1` if negative, `+1` if positive.  
Should be called only if `this != 0`.  
In the `0` case, this will not terminate correctly; typically it  
will run until it exhausts memory.  
If the two constructive reals may be equal, the one or two argument  
version of Sign should be used.