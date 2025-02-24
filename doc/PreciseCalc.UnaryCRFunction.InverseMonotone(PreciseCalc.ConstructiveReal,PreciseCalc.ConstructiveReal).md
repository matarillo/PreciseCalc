### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[UnaryCRFunction](PreciseCalc.UnaryCRFunction.md 'PreciseCalc.UnaryCRFunction')

## UnaryCRFunction.InverseMonotone(ConstructiveReal, ConstructiveReal) Method

Computes the inverse of this function on the given interval.

```csharp
public PreciseCalc.UnaryCRFunction InverseMonotone(PreciseCalc.ConstructiveReal low, PreciseCalc.ConstructiveReal high);
```
#### Parameters

<a name='PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low'></a>

`low` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

Lower bound of the interval

<a name='PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high'></a>

`high` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

Upper bound of the interval

#### Returns
[UnaryCRFunction](PreciseCalc.UnaryCRFunction.md 'PreciseCalc.UnaryCRFunction')  
The inverse function as a UnaryCRFunction

### Remarks
Compute the inverse of this function, which must be defined  
and strictly monotone on the interval [[low](PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low 'PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).low'), [high](PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high 'PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).high')].  
The resulting function is defined only on the image of  
[[low](PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low 'PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).low'), [high](PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high 'PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).high')].  
The original function may be either increasing or decreasing.