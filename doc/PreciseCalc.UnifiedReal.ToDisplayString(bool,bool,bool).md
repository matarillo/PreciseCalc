### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

## UnifiedReal.ToDisplayString(bool, bool, bool) Method

Convert to readable String. Intended for user output. Produces exact expression when possible.  
If degrees is true, then any trig functions in the output will refer to the degree versions.  
Otherwise, the radian versions are used.

```csharp
public string ToDisplayString(bool degrees, bool unicodeFraction, bool mixed);
```
#### Parameters

<a name='PreciseCalc.UnifiedReal.ToDisplayString(bool,bool,bool).degrees'></a>

`degrees` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

<a name='PreciseCalc.UnifiedReal.ToDisplayString(bool,bool,bool).unicodeFraction'></a>

`unicodeFraction` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

<a name='PreciseCalc.UnifiedReal.ToDisplayString(bool,bool,bool).mixed'></a>

`mixed` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')