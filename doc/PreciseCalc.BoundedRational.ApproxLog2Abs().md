### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')

## BoundedRational.ApproxLog2Abs() Method

Returns an approximation of the base 2 log of the absolute value.  
We assume this is nonzero.  
The result is either 0 or within 20% of the truth.

```csharp
public double ApproxLog2Abs();
```

#### Returns
[System.Double](https://docs.microsoft.com/en-us/dotnet/api/System.Double 'System.Double')

#### Exceptions

[System.InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException 'System.InvalidOperationException')  
When invalid