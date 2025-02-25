### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

## UnifiedReal.LeadingBinaryZeroes() Method

Return an upper bound on the number of leading zero bits. These are the number of 0 bits to the  
right of the binary point and to the left of the most significant digit. Return  
Integer.MAX_VALUE if we cannot bound it based only on the rational factor and property.

```csharp
public int LeadingBinaryZeroes();
```

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')