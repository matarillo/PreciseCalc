### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')

## BoundedRational.ToDouble() Method

Return a double approximation.

```csharp
public double ToDouble();
```

#### Returns
[System.Double](https://docs.microsoft.com/en-us/dotnet/api/System.Double 'System.Double')

#### Exceptions

[System.InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException 'System.InvalidOperationException')  
When invalid

### Remarks
The result is correctly rounded to nearest, with ties rounded away from zero.  
TODO: Should round ties to even.