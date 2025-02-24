### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.Approximate(int) Method

Subclasses must implement this method to approximate the value to a given precision.

```csharp
protected abstract System.Numerics.BigInteger Approximate(int precision);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.Approximate(int).precision'></a>

`precision` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

#### Returns
[System.Numerics.BigInteger](https://docs.microsoft.com/en-us/dotnet/api/System.Numerics.BigInteger 'System.Numerics.BigInteger')

### Remarks
Most users can ignore the existence of this method, and will  
not ever need to define a subclass of [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal').  
Returns value / 2 ** [precision](PreciseCalc.ConstructiveReal.Approximate(int).md#PreciseCalc.ConstructiveReal.Approximate(int).precision 'PreciseCalc.ConstructiveReal.Approximate(int).precision') rounded to an integer.  
The error in the result is strictly < 1.  
Informally, Approximate(n) gives a scaled approximation  
accurate to 2**n.  
Implementations may safely assume that [precision](PreciseCalc.ConstructiveReal.Approximate(int).md#PreciseCalc.ConstructiveReal.Approximate(int).precision 'PreciseCalc.ConstructiveReal.Approximate(int).precision') is  
at least a factor of 8 away from overflow.  
Called only with the lock on the [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal') object  
already held.