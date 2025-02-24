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

| Fields | |
| :--- | :--- |
| [AbsFunction](PreciseCalc.UnaryCRFunction.AbsFunction.md 'PreciseCalc.UnaryCRFunction.AbsFunction') | The absolute value function f(x) = |x|. |
| [AcosFunction](PreciseCalc.UnaryCRFunction.AcosFunction.md 'PreciseCalc.UnaryCRFunction.AcosFunction') | The arccosine function f(x) = acos(x). |
| [AsinFunction](PreciseCalc.UnaryCRFunction.AsinFunction.md 'PreciseCalc.UnaryCRFunction.AsinFunction') | The arcsine function f(x) = asin(x). |
| [AtanFunction](PreciseCalc.UnaryCRFunction.AtanFunction.md 'PreciseCalc.UnaryCRFunction.AtanFunction') | The arctangent function f(x) = atan(x). |
| [CosFunction](PreciseCalc.UnaryCRFunction.CosFunction.md 'PreciseCalc.UnaryCRFunction.CosFunction') | The cosine function f(x) = cos(x). |
| [ExpFunction](PreciseCalc.UnaryCRFunction.ExpFunction.md 'PreciseCalc.UnaryCRFunction.ExpFunction') | The exponential function f(x) = exp(x). |
| [IdentityFunction](PreciseCalc.UnaryCRFunction.IdentityFunction.md 'PreciseCalc.UnaryCRFunction.IdentityFunction') | The identity function f(x) = x. |
| [InverseFunction](PreciseCalc.UnaryCRFunction.InverseFunction.md 'PreciseCalc.UnaryCRFunction.InverseFunction') | The inverse function f(x) = 1/x. |
| [LnFunction](PreciseCalc.UnaryCRFunction.LnFunction.md 'PreciseCalc.UnaryCRFunction.LnFunction') | The natural logarithm function f(x) = ln(x). |
| [NegateFunction](PreciseCalc.UnaryCRFunction.NegateFunction.md 'PreciseCalc.UnaryCRFunction.NegateFunction') | The negation function f(x) = -x. |
| [SinFunction](PreciseCalc.UnaryCRFunction.SinFunction.md 'PreciseCalc.UnaryCRFunction.SinFunction') | The sine function f(x) = sin(x). |
| [SqrtFunction](PreciseCalc.UnaryCRFunction.SqrtFunction.md 'PreciseCalc.UnaryCRFunction.SqrtFunction') | The square root function f(x) = sqrt(x). |
| [TanFunction](PreciseCalc.UnaryCRFunction.TanFunction.md 'PreciseCalc.UnaryCRFunction.TanFunction') | The tangent function f(x) = tan(x). |

| Methods | |
| :--- | :--- |
| [Compose(UnaryCRFunction)](PreciseCalc.UnaryCRFunction.Compose(PreciseCalc.UnaryCRFunction).md 'PreciseCalc.UnaryCRFunction.Compose(PreciseCalc.UnaryCRFunction)') | Composes this function with another function. |
| [Execute(ConstructiveReal)](PreciseCalc.UnaryCRFunction.Execute(PreciseCalc.ConstructiveReal).md 'PreciseCalc.UnaryCRFunction.Execute(PreciseCalc.ConstructiveReal)') | Computes the function result for a given [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal'). |
| [InverseMonotone(ConstructiveReal, ConstructiveReal)](PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md 'PreciseCalc.UnaryCRFunction.InverseMonotone(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal)') | Computes the inverse of this function on the given interval. |
| [MonotoneDerivative(ConstructiveReal, ConstructiveReal)](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal)') | Compute the derivative of a function.<br/>The function must be defined on the interval [[low](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).low'), [high](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).high')],<br/>and the derivative must exist, and must be continuous and<br/>monotone in the open interval [[low](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).low 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).low'), [high](PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).md#PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal,PreciseCalc.ConstructiveReal).high 'PreciseCalc.UnaryCRFunction.MonotoneDerivative(PreciseCalc.ConstructiveReal, PreciseCalc.ConstructiveReal).high')].<br/>The result is defined only in the open interval. |
