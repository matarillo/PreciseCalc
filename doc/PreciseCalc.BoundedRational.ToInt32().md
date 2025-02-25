### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')

## BoundedRational.ToInt32() Method

Returns the integer value of this rational number.  
Throws an exception if this is not an integer.

```csharp
public int ToInt32();
```

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

#### Exceptions

[System.InvalidOperationException](https://docs.microsoft.com/en-us/dotnet/api/System.InvalidOperationException 'System.InvalidOperationException')  
When invalid

[System.ArithmeticException](https://docs.microsoft.com/en-us/dotnet/api/System.ArithmeticException 'System.ArithmeticException')  
IntValue of non-int