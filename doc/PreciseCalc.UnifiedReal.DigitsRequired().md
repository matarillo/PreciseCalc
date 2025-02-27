### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

## UnifiedReal.DigitsRequired() Method

Return the number of decimal digits to the right of the decimal point required to represent  
the argument exactly. Return Integer.MAX_VALUE if that's not possible. Never returns a value  
less than zero, even if r is a power of ten.

```csharp
public int DigitsRequired();
```

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')