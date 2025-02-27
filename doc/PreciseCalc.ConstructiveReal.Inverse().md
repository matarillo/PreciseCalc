### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.Inverse() Method

Returns the multiplicative inverse of a constructive real number.

```csharp
public PreciseCalc.ConstructiveReal Inverse();
```

#### Returns
[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

### Remarks
`x.Inverse()` is equivalent to `ConstructiveReal.FromInt(1).Divide(x)`.