### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[UnifiedReal](PreciseCalc.UnifiedReal.md 'PreciseCalc.UnifiedReal')

## UnifiedReal.ToStringTruncated(int) Method

Returns a truncated representation of the result.  
If [ExactlyTruncatable()](PreciseCalc.UnifiedReal.ExactlyTruncatable().md 'PreciseCalc.UnifiedReal.ExactlyTruncatable()'), we round correctly towards zero. Otherwise, the resulting digit  
string may occasionally be rounded up instead.  
Always includes a decimal point in the result.  
The result includes n digits to the right of the decimal point.

```csharp
public string ToStringTruncated(int n);
```
#### Parameters

<a name='PreciseCalc.UnifiedReal.ToStringTruncated(int).n'></a>

`n` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

result precision, >= 0

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')