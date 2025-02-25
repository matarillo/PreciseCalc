### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

## UnifiedReal.DefinitelyIndependent(UnifiedReal) Method

Do we know that this.crFactor is an irrational nonzero multiple of u.crFactor? If this returns  
true, then a comparison of the two UnifiedReals cannot diverge, though we don't know of a good  
runtime bound. Note that if both values are really tiny, it still may be completely impractical  
to compare them.

```csharp
public bool DefinitelyIndependent(PreciseCalc.UnifiedReal u);
```
#### Parameters

<a name='PreciseCalc.UnifiedReal.DefinitelyIndependent(PreciseCalc.UnifiedReal).u'></a>

`u` [UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')