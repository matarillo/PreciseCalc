### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')

## BoundedRational.DigitsRequired(BoundedRational) Method

Computes the number of decimal digits required for exact representation.

```csharp
public static int DigitsRequired(PreciseCalc.BoundedRational r);
```
#### Parameters

<a name='PreciseCalc.BoundedRational.DigitsRequired(PreciseCalc.BoundedRational).r'></a>

`r` [BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

### Remarks
Return the number of decimal digits to the right of the decimal point  
required to represent the argument exactly.  
Return [System.Int32.MaxValue](https://docs.microsoft.com/en-us/dotnet/api/System.Int32.MaxValue 'System.Int32.MaxValue') if that's not possible.  
Never returns a value less than zero, even if r is a power of ten.