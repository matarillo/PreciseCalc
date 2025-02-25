### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')

## BoundedRational.DigitsRequired Property

Computes the number of decimal digits required for exact representation.

```csharp
public int DigitsRequired { get; }
```

#### Property Value
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

### Remarks
Return the number of decimal digits to the right of the decimal point  
required to represent the argument exactly.  
Return [System.Int32.MaxValue](https://docs.microsoft.com/en-us/dotnet/api/System.Int32.MaxValue 'System.Int32.MaxValue') if that's not possible.  
Never returns a value less than zero, even if r is a power of ten.