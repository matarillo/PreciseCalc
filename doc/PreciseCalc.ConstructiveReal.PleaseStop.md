### [PreciseCalc](PreciseCalc.md 'PreciseCalc').[ConstructiveReal](PreciseCalc.ConstructiveReal.md 'PreciseCalc.ConstructiveReal')

## ConstructiveReal.PleaseStop Field

Setting this to true requests that all computations be aborted by  
throwing [System.OperationCanceledException](https://docs.microsoft.com/en-us/dotnet/api/System.OperationCanceledException 'System.OperationCanceledException').  
Must be rest to false before any further computation.

```csharp
public static volatile bool PleaseStop;
```

#### Field Value
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')