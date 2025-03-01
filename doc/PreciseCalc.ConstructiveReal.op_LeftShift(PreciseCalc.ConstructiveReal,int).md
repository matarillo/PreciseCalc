### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.operator <<(ConstructiveReal, int) Operator

Shifts left by n bits.

```csharp
public static PreciseCalc.ConstructiveReal operator <<(PreciseCalc.ConstructiveReal value, int n);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal,int).value'></a>

`value` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

The value that is shifted left by [n](PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal,int).md#PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal,int).n 'PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal, int).n').

<a name='PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal,int).n'></a>

`n` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

shift count, may be negative

#### Returns
[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

### Remarks
Multiply a constructive real by 2**[n](PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal,int).md#PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal,int).n 'PreciseCalc.ConstructiveReal.op_LeftShift(PreciseCalc.ConstructiveReal, int).n').