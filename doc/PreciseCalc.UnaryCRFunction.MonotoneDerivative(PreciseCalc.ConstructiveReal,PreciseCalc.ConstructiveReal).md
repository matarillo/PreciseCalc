### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[UnaryCRFunction](PreciseCalc.UnaryCRFunction.md 'PreciseCalc.UnaryCRFunction')

## UnaryCRFunction.MonotoneDerivative(ConstructiveReal, ConstructiveReal) Method

Compute the derivative of a function.  
The function must be defined on the interval [[low](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).low'), [high](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).high')],  
and the derivative must exist, and must be continuous and  
monotone in the open interval [[low](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).low'), [high](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).high')].  
The result is defined only in the open interval.

```csharp
public PreciseCalc.UnaryCRFunction MonotoneDerivative(PreciseCalc.ConstructiveReal low, PreciseCalc.ConstructiveReal high);
```
#### Parameters

<a name='PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low'></a>

`low` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

Lower bound of the interval

<a name='PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high'></a>

`high` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

Upper bound of the interval

#### Returns
[UnaryCRFunction](PreciseCalc.UnaryCRFunction.md 'PreciseCalc.UnaryCRFunction')  
The derivative function as a UnaryCRFunction