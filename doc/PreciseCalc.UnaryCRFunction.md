### [PreciseCalc](PreciseCalc.md 'PreciseCalc')

## UnaryCRFunction Class

Represents a unary function on constructive reals.

```csharp
public abstract class UnaryCRFunction
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; UnaryCRFunction

### Remarks
The [Execute(ConstructiveReal)](PreciseCalc.UnaryCRFunction.Execute(PreciseCalc.ConstructiveReal).md 'PreciseCalc.UnaryCRFunction.Execute(PreciseCalc.ConstructiveReal)') method computes the function result.  
Subclasses should implement specific mathematical functions.

| Methods | |
| :--- | :--- |
| [Compose(UnaryCRFunction)](PreciseCalc.UnaryCRFunction.Compose(PreciseCalc.UnaryCRFunction).md 'PreciseCalc.UnaryCRFunction.Compose(PreciseCalc.UnaryCRFunction)') | Composes this function with another function. |
| [Execute(ConstructiveReal)](PreciseCalc.UnaryCRFunction.Execute(PreciseCalc.ConstructiveReal).md 'PreciseCalc.UnaryCRFunction.Execute(PreciseCalc.ConstructiveReal)') | Computes the function result for a given [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal'). |
| [InverseMonotone(ConstructiveReal, ConstructiveReal)](PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md 'PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal)') | Computes the inverse of this function on the given interval. |
| [MonotoneDerivative(ConstructiveReal, ConstructiveReal)](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal)') | Compute the derivative of a function.<br/>The function must be defined on the interval [[low](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).low'), [high](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).high')],<br/>and the derivative must exist, and must be continuous and<br/>monotone in the open interval [[low](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).low'), [high](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).high')].<br/>The result is defined only in the open interval. |
