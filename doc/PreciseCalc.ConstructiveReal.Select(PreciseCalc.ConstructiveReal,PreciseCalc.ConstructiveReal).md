### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.Select(ConstructiveReal, ConstructiveReal) Method

Selects one of two constructive reals based on the sign of this value.

```csharp
public PreciseCalc.ConstructiveReal Select(PreciseCalc.ConstructiveReal x, PreciseCalc.ConstructiveReal y);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).x'></a>

`x` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

<a name='PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).y'></a>

`y` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

#### Returns
[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

### Remarks
The real number [x](PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).x 'PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).x') if `this` < 0, or [y](PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).y 'PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).y') otherwise.  
Requires [x](PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).x 'PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).x') = [y](PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).y 'PreciseCalc.ConstructiveReal.Select(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).y') if `this` = 0.  
Since comparisons may diverge, this is often  
a useful alternative to conditionals.