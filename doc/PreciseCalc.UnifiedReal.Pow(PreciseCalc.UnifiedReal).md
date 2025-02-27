### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

## UnifiedReal.Pow(UnifiedReal) Method

Return this ^ exp. This is really only well-defined for a positive base, particularly since  
0^x is not continuous at zero. 0^0 = 1 (as is epsilon^0), but 0^epsilon is 0. We nonetheless  
try to do reasonable things at zero, when we recognize that case.

```csharp
public PreciseCalc.UnifiedReal Pow(PreciseCalc.UnifiedReal exp);
```
#### Parameters

<a name='PreciseCalc.UnifiedReal.Pow(PreciseCalc.UnifiedReal).exp'></a>

`exp` [UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

#### Returns
[UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')