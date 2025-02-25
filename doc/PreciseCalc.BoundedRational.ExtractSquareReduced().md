### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')

## BoundedRational.ExtractSquareReduced() Method

Returns a pair p such that p[0]^2 * p[1] = this.  
Tries to maximize p[0]. This rational is assumed to be in reduced form.

```csharp
public PreciseCalc.BoundedRational[] ExtractSquareReduced();
```

#### Returns
[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')[[]](https://docs.microsoft.com/en-us/dotnet/api/System.Array 'System.Array')

#### Exceptions

[System.InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException 'System.InvalidOperationException')  
When invalid