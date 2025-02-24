### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.AtanPI Field

Our old PI implementation. pi/4 = 4*atan(1/5) - atan(1/239)

```csharp
public static readonly ConstructiveReal AtanPI;
```

#### Field Value
[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

### Remarks
Our old PI implementation. Keep this around for now to allow checking.  
This implementation may also be faster for BigInteger implementations  
that support only quadratic multiplication, but exhibit high performance  
for small computations.  (The standard Android 6 implementation supports  
subquadratic multiplication, but has high constant overhead.) Many other  
atan-based formulas are possible, but based on superficial  
experimentation, this is roughly as good as the more complex formulas.