### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.CompareTo(ConstructiveReal, int, int) Method

Compares this value with another constructive real, refining precision iteratively.

```csharp
public int CompareTo(PreciseCalc.ConstructiveReal x, int relPrecision, int absPrecision);
```
#### Parameters

<a name='PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int,int).x'></a>

`x` [ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

The other constructive real

<a name='PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int,int).relPrecision'></a>

`relPrecision` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Relative tolerance in bits

<a name='PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int,int).absPrecision'></a>

`absPrecision` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

Absolute tolerance in bits

#### Returns
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

### Remarks
Return 0 if x = y to within the indicated tolerance,  
-1 if x < y, and +1 if x > y.  If x and y are indeed  
equal, it is guaranteed that 0 will be returned.  If  
they differ by less than the tolerance, anything  
may happen.  The tolerance allowed is the maximum of  
(Abs(this)+Abs(x))*(2**[relPrecision](PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int,int).md#PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int,int).relPrecision 'PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal, int, int).relPrecision')) and 2**[absPrecision](PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int,int).md#PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal,int,int).absPrecision 'PreciseCalc.ConstructiveReal.CompareTo(PreciseCalc.ConstructiveReal, int, int).absPrecision').