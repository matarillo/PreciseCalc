### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[BoundedRational](PreciseCalc.BoundedRational.md 'PreciseCalc.BoundedRational')

## BoundedRational.ToDisplayString(bool, bool) Method

Converts to a readable string representation. Renamed from toNiceString.

```csharp
public string ToDisplayString(bool useUnicodeFractions=false, bool useMixedNumbers=false);
```
#### Parameters

<a name='PreciseCalc.BoundedRational.ToDisplayString(bool,bool).useUnicodeFractions'></a>

`useUnicodeFractions` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

If true, use subscript and superscript characters to represent the fraction.

<a name='PreciseCalc.BoundedRational.ToDisplayString(bool,bool).useMixedNumbers'></a>

`useMixedNumbers` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

If true, convert improper fractions to mixed fractions.

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

### Remarks
Intended for output to user. More expensive, less useful for debugging than ToString().  
[useUnicodeFractions](PreciseCalc.BoundedRational.ToDisplayString(bool,bool).md#PreciseCalc.BoundedRational.ToDisplayString(bool,bool).useUnicodeFractions 'PreciseCalc.BoundedRational.ToDisplayString(bool, bool).useUnicodeFractions') relies on Unicode characters intended for this purpose.  
Not internationalized.