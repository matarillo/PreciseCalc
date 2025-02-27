### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.AssumeInt() Method

Assumes the constructive real is an integer, preventing unnecessary evaluations.

```csharp
public PreciseCalc.ConstructiveReal AssumeInt();
```

#### Returns
[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

### Remarks
Produce a constructive real equivalent to the original, assuming  
the original was an integer.  Undefined results if the original  
was not an integer.  Prevents evaluation of digits to the right  
of the decimal point, and may thus improve performance.