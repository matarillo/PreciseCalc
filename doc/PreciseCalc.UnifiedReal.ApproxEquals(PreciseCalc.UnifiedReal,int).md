### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

## UnifiedReal.ApproxEquals(UnifiedReal, int) Method

Equality comparison. May erroneously return true if values differ by less than 2^a, and  
!isComparable(u).

```csharp
public bool ApproxEquals(PreciseCalc.UnifiedReal u, int a);
```
#### Parameters

<a name='PreciseCalc.UnifiedReal.ApproxEquals(PreciseCalc.UnifiedReal,int).u'></a>

`u` [UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

<a name='PreciseCalc.UnifiedReal.ApproxEquals(PreciseCalc.UnifiedReal,int).a'></a>

`a` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')