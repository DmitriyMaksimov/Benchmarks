# NullLoggerWithFunction
Another common use of logging when we are using a serializer to convert a DTO to string. 

Example:
```csharp
private class DtoModel
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

...
var dto = new DtoModel {Id = 42, Message = "Hello, World!", CreatedAt = DateTime.UtcNow};
logger.LogDebug("DTO: {Dto}", JsonSerializer.Serialize(dto))
```

The above code has an issue - `JsonSerializer.Serialize(dto)` call every time, even if the logging is not required (because `MinLoggingLevel` set to `Information`).

We can improve performance by deferring evaluation of `JsonSerializer.Serialize(dto)` call.

Example of a helper method:
```csharp
public static void Debug<T0>(this ILogger logger, [StructuredMessageTemplate] string messageTemplate, Func<T0> func)
{
    if (logger.IsEnabled(LogLevel.Debug))
    {
        logger.Log(LogLevel.Debug, messageTemplate, func());
    }
}
```

Usage of the helper method very similar to normal logging (just need to wrap the argument to a lambda `() => `):
```csharp
logger.LogDebug("DTO: {Dto}", () => JsonSerializer.Serialize(dto))
```

Downside of the helper method that it always creat an instance of `Func<T>()`.

Does the downside overweight cost is too high? Let's measure it! 

## Methodology
Tested logging with one `string` parameter.

There are 2 variations of tests:
- `DtoModel` a simple DTO model with 1 reference and 2 value types
- `DtoModel2` a DTO model with 1 reference, 2 value types and array of `DtoModel`

## Conclusions
- Deferring `JsonSerializer.Serialize(dto)` makes logging significantly faster when logging is not performed (not a surprise :grin:)  
- Creating instance of `Func<string>()` allocates 64 bytes of memory
- Using `[LoggerMessage]` with a "heavy" parameters (like `JsonSerializer.Serialize(dto)`) has no workarounds (except writing wrappers for each case which doesn't sounds like a great idea) 

## Benchmark results

### Mac
```
BenchmarkDotNet v0.13.12, macOS Sonoma 14.4.1 (23E224) [Darwin 23.4.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
  Job-BAIQBG : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD

Server=False  
```
| Method                              | Categories   | N      | Mean              | Error           | StdDev          | CI99.9% Margin  | Ratio    | RatioSD | Rank | Gen0       | Allocated  | Alloc Ratio |
|------------------------------------ |------------- |------- |------------------:|----------------:|----------------:|----------------:|---------:|--------:|-----:|-----------:|-----------:|------------:|
| HelperWithComplexModel1             | ComplexModel | 1      |        524.707 ns |       3.9407 ns |       3.6862 ns |       3.9407 ns | +10,498% |    2.0% |    2 |     0.1116 |      936 B |     +1,362% |
| HelperWithFunctionWithComplexModel1 | ComplexModel | 1      |          4.952 ns |       0.0819 ns |       0.0766 ns |       0.0819 ns | baseline |         |    1 |     0.0076 |       64 B |             |
| SourceGeneratorWithComplexModel1    | ComplexModel | 1      |        546.384 ns |       5.5127 ns |       5.1566 ns |       5.5127 ns | +10,935% |    1.7% |    3 |     0.1116 |      936 B |     +1,362% |
|                                     |              |        |                   |                 |                 |                 |          |         |      |            |            |             |
| Helper1                             | SimpleModel  | 1      |        154.833 ns |       3.0923 ns |       3.4370 ns |       3.0923 ns |  +2,945% |    2.1% |    3 |     0.0210 |      176 B |       +175% |
| HelperWithFunction1                 | SimpleModel  | 1      |          5.108 ns |       0.0891 ns |       0.0744 ns |       0.0891 ns | baseline |         |    1 |     0.0076 |       64 B |             |
| SourceGenerator1                    | SimpleModel  | 1      |        138.790 ns |       0.5464 ns |       0.4563 ns |       0.5464 ns |  +2,618% |    1.5% |    2 |     0.0210 |      176 B |       +175% |
|                                     |              |        |                   |                 |                 |                 |          |         |      |            |            |             |
| HelperWithComplexModel1             | ComplexModel | 1000   |    562,296.024 ns |   5,164.5772 ns |   4,830.9487 ns |   5,164.5772 ns | +10,799% |    0.9% |    3 |   111.3281 |   936017 B |     +1,363% |
| HelperWithFunctionWithComplexModel1 | ComplexModel | 1000   |      5,159.439 ns |      32.0715 ns |      29.9997 ns |      32.0715 ns | baseline |         |    1 |     7.6447 |    64000 B |             |
| SourceGeneratorWithComplexModel1    | ComplexModel | 1000   |    528,692.091 ns |   2,801.7292 ns |   2,620.7392 ns |   2,801.7292 ns | +10,147% |    0.8% |    2 |   111.3281 |   936017 B |     +1,363% |
|                                     |              |        |                   |                 |                 |                 |          |         |      |            |            |             |
| Helper1                             | SimpleModel  | 1000   |    143,882.117 ns |     764.6713 ns |     715.2740 ns |     764.6713 ns |  +2,600% |    0.7% |    2 |    20.9961 |   176004 B |       +175% |
| HelperWithFunction1                 | SimpleModel  | 1000   |      5,328.679 ns |      34.3047 ns |      30.4102 ns |      34.3047 ns | baseline |         |    1 |     7.6447 |    64000 B |             |
| SourceGenerator1                    | SimpleModel  | 1000   |    152,070.610 ns |   2,006.1342 ns |   1,876.5392 ns |   2,006.1342 ns |  +2,750% |    1.4% |    3 |    20.9961 |   176004 B |       +175% |
|                                     |              |        |                   |                 |                 |                 |          |         |      |            |            |             |
| HelperWithComplexModel1             | ComplexModel | 100000 | 56,826,102.979 ns | 515,529.2983 ns | 457,003.2950 ns | 515,529.2983 ns | +10,624% |    1.9% |    2 | 11100.0000 | 93601714 B |     +1,363% |
| HelperWithFunctionWithComplexModel1 | ComplexModel | 100000 |    530,057.789 ns |  10,582.2724 ns |   9,380.9089 ns |  10,582.2724 ns | baseline |         |    1 |   764.6484 |  6400001 B |             |
| SourceGeneratorWithComplexModel1    | ComplexModel | 100000 | 56,938,903.167 ns | 734,361.1851 ns | 573,340.9091 ns | 734,361.1851 ns | +10,643% |    2.2% |    2 | 11111.1111 | 93601905 B |     +1,363% |
|                                     |              |        |                   |                 |                 |                 |          |         |      |            |            |             |
| Helper1                             | SimpleModel  | 100000 | 14,385,607.119 ns |  88,909.1844 ns |  83,165.7063 ns |  88,909.1844 ns |  +2,437% |    1.0% |    2 |  2093.7500 | 17600268 B |       +175% |
| HelperWithFunction1                 | SimpleModel  | 100000 |    567,672.554 ns |   4,613.4963 ns |   3,852.4783 ns |   4,613.4963 ns | baseline |         |    1 |   764.6484 |  6400001 B |             |
| SourceGenerator1                    | SimpleModel  | 100000 | 14,407,950.955 ns | 141,718.7973 ns | 132,563.8509 ns | 141,718.7973 ns |  +2,433% |    0.7% |    2 |  2093.7500 | 17600268 B |       +175% |
