# NullLogger
Same tests as [Logging](Logging.md), but using `NullLogger`.

The purpose of the benchmark is to understand if using of `NullLogger` is acceptable is a tough loops.

To better understand the topic, I'll give you an example.

I'm comparing the following
```csharp
class MyClass
{
    private ILogger<MyClass> logger;
    private bool verboseLoggingEnabled = false;
    
    public MyClass(ILogger<MyClass> logger)
    {
        verboseLoggingEnabled = false;  // Value of this field based on some business logic
        this.logger = logger;
    }
    
    public void DoWork(IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            // Log item if versobe logging enabled
            if (verboseLoggingEnabled)
            {
                logger.LogInformation("{Item}", item);
            }
            // process item
        }
    }
}
```
with
```csharp
class MyClass
{
    private ILogger<MyClass> logger;
    private ILogger<MyClass> verboseLogger;
    private bool verboseLoggingEnabled = false;
    
    public MyClass(ILogger<MyClass> logger)
    {
        verboseLoggingEnabled = false;  // Value of this field based on some business logic
        this.logger = logger;
        verboseLogger = verboseLoggingEnabled ? logger : NullLoggerFactory.Instance.CreateLogger<MyClass>();
    }
    
    public void DoWork(IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            verboseLogger.LogInformation("{Item}", item);
            // process item
        }
    }
}
```

IMO, the latter is nicer - you don't need to surround all verbose logging with `if (verboseLoggingEnabled)`.

The question is it too slow and we should stick with the former variant.


## Methodology
Tested logging without, with one, two and three parameters.

There are 2 variations of tests with 3 parameters:
- `int`, `string` and `DateTime` (1 reference and 2 value types)
- all parameters of type `string` (reference type) 

## Conclusions
  - `MEL` is the slowest method. It is slow - up to 48 times!
  - All other logging methods are about the same as `SourceGenerator`
  - There are no memory allocations and no boxing/unboxing (again, except `MEL`)
  - Logging level has no impact. No surprises here - `NullLogger.IsEnabled()` unconditionally return `false`

```csharp
public bool IsEnabled(LogLevel logLevel) => false;
```

Is using of `NullLogger` is acceptable is a tough loops?

The answer is "It depends".

All logging methods except `MEL` are under 1ns (1 Nanosecond = 0.000000001 sec), so if you loop thru 1,000,000 items, logging impact would be around 1ms.
To confirm it, I created [NullLoggerInsideLoop](../Benchmarks/NullLoggerInsideLoop.cs) test.

According to the test
- looping thru 1,000,000,000 items with `NullLogger` takes ~0.6ms
- looping thru 1,000,000,000,000 items with `NullLogger` takes ~1.5 seconds

## Benchmark results

### Mac - all logging methods
```
BenchmarkDotNet v0.13.12, macOS Sonoma 14.4.1 (23E224) [Darwin 23.4.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
  Job-YRPLSP : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD

Server=False  
```

| Method              | Categories | MinLogLevel | Mean       | Error     | StdDev    | CI99.9% Margin | Ratio    | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|-------------------- |----------- |------------ |-----------:|----------:|----------:|---------------:|---------:|--------:|-----:|-------:|----------:|------------:|
| MEL0                | 0          | Debug       |  9.5116 ns | 0.0348 ns | 0.0308 ns |      0.0348 ns |  +1,253% |    0.8% |    4 |      - |         - |          NA |
| IsEnabledCheck0     | 0          | Debug       |  0.6409 ns | 0.0047 ns | 0.0044 ns |      0.0047 ns |      -9% |    0.9% |    2 |      - |         - |          NA |
| Helper0             | 0          | Debug       |  0.6290 ns | 0.0052 ns | 0.0049 ns |      0.0052 ns |     -11% |    1.1% |    1 |      - |         - |          NA |
| SourceGenerator0    | 0          | Debug       |  0.7035 ns | 0.0046 ns | 0.0043 ns |      0.0046 ns | baseline |         |    3 |      - |         - |          NA |
|                     |            |             |            |           |           |                |          |         |      |        |           |             |
| MEL1                | 1          | Debug       | 26.1482 ns | 0.0841 ns | 0.0787 ns |      0.0841 ns |  +4,104% |    0.8% |    3 | 0.0067 |      56 B |          NA |
| IsEnabledCheck1     | 1          | Debug       |  0.6265 ns | 0.0059 ns | 0.0055 ns |      0.0059 ns |      +1% |    1.5% |    1 |      - |         - |          NA |
| Helper1             | 1          | Debug       |  0.6859 ns | 0.0041 ns | 0.0039 ns |      0.0041 ns |     +10% |    0.8% |    2 |      - |         - |          NA |
| SourceGenerator1    | 1          | Debug       |  0.6221 ns | 0.0050 ns | 0.0047 ns |      0.0050 ns | baseline |         |    1 |      - |         - |          NA |
|                     |            |             |            |           |           |                |          |         |      |        |           |             |
| MEL2                | 2          | Debug       | 28.0768 ns | 0.0822 ns | 0.0769 ns |      0.0822 ns |  +4,393% |    1.0% |    3 | 0.0076 |      64 B |          NA |
| IsEnabledCheck2     | 2          | Debug       |  0.6182 ns | 0.0046 ns | 0.0043 ns |      0.0046 ns |      -1% |    0.9% |    1 |      - |         - |          NA |
| Helper2             | 2          | Debug       |  0.6682 ns | 0.0046 ns | 0.0041 ns |      0.0046 ns |      +7% |    1.0% |    2 |      - |         - |          NA |
| SourceGenerator2    | 2          | Debug       |  0.6249 ns | 0.0061 ns | 0.0057 ns |      0.0061 ns | baseline |         |    1 |      - |         - |          NA |
|                     |            |             |            |           |           |                |          |         |      |        |           |             |
| MEL3                | 3          | Debug       | 32.4102 ns | 0.1632 ns | 0.1527 ns |      0.1632 ns |  +4,827% |    1.0% |    7 | 0.0114 |      96 B |          NA |
| MELRef3             | 3          | Debug       | 29.5976 ns | 0.1171 ns | 0.1096 ns |      0.1171 ns |  +4,400% |    0.8% |    6 | 0.0057 |      48 B |          NA |
| IsEnabledCheck3     | 3          | Debug       |  0.6136 ns | 0.0029 ns | 0.0026 ns |      0.0029 ns |      -7% |    0.9% |    1 |      - |         - |          NA |
| Helper3             | 3          | Debug       |  0.7321 ns | 0.0052 ns | 0.0049 ns |      0.0052 ns |     +11% |    1.1% |    5 |      - |         - |          NA |
| HelperRef3          | 3          | Debug       |  0.6890 ns | 0.0050 ns | 0.0046 ns |      0.0050 ns |      +5% |    0.9% |    4 |      - |         - |          NA |
| SourceGenerator3    | 3          | Debug       |  0.6578 ns | 0.0055 ns | 0.0052 ns |      0.0055 ns | baseline |         |    3 |      - |         - |          NA |
| SourceGeneratorRef3 | 3          | Debug       |  0.6248 ns | 0.0063 ns | 0.0059 ns |      0.0063 ns |      -5% |    1.4% |    2 |      - |         - |          NA |
|                     |            |             |            |           |           |                |          |         |      |        |           |             |
| MEL0                | 0          | Information |  9.5714 ns | 0.0195 ns | 0.0163 ns |      0.0195 ns |  +1,272% |    0.5% |    3 |      - |         - |          NA |
| IsEnabledCheck0     | 0          | Information |  0.6193 ns | 0.0057 ns | 0.0053 ns |      0.0057 ns |     -11% |    0.9% |    1 |      - |         - |          NA |
| Helper0             | 0          | Information |  0.6254 ns | 0.0056 ns | 0.0053 ns |      0.0056 ns |     -11% |    1.0% |    1 |      - |         - |          NA |
| SourceGenerator0    | 0          | Information |  0.6983 ns | 0.0042 ns | 0.0038 ns |      0.0042 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |             |            |           |           |                |          |         |      |        |           |             |
| MEL1                | 1          | Information | 25.9978 ns | 0.0670 ns | 0.0523 ns |      0.0670 ns |  +4,067% |    0.7% |    2 | 0.0067 |      56 B |          NA |
| IsEnabledCheck1     | 1          | Information |  0.6230 ns | 0.0095 ns | 0.0089 ns |      0.0095 ns |      -0% |    1.4% |    1 |      - |         - |          NA |
| Helper1             | 1          | Information |  0.6164 ns | 0.0038 ns | 0.0034 ns |      0.0038 ns |      -1% |    0.9% |    1 |      - |         - |          NA |
| SourceGenerator1    | 1          | Information |  0.6238 ns | 0.0040 ns | 0.0037 ns |      0.0040 ns | baseline |         |    1 |      - |         - |          NA |
|                     |            |             |            |           |           |                |          |         |      |        |           |             |
| MEL2                | 2          | Information | 27.7237 ns | 0.0767 ns | 0.0717 ns |      0.0767 ns |  +4,404% |    1.0% |    2 | 0.0076 |      64 B |          NA |
| IsEnabledCheck2     | 2          | Information |  0.6218 ns | 0.0062 ns | 0.0058 ns |      0.0062 ns |      +1% |    1.4% |    1 |      - |         - |          NA |
| Helper2             | 2          | Information |  0.6200 ns | 0.0048 ns | 0.0044 ns |      0.0048 ns |      +1% |    1.2% |    1 |      - |         - |          NA |
| SourceGenerator2    | 2          | Information |  0.6156 ns | 0.0061 ns | 0.0057 ns |      0.0061 ns | baseline |         |    1 |      - |         - |          NA |
|                     |            |             |            |           |           |                |          |         |      |        |           |             |
| MEL3                | 3          | Information | 31.9283 ns | 0.1394 ns | 0.1304 ns |      0.1394 ns |  +4,620% |    1.0% |    5 | 0.0114 |      96 B |          NA |
| MELRef3             | 3          | Information | 29.8674 ns | 0.2111 ns | 0.1975 ns |      0.2111 ns |  +4,315% |    0.7% |    4 | 0.0057 |      48 B |          NA |
| IsEnabledCheck3     | 3          | Information |  0.6352 ns | 0.0051 ns | 0.0047 ns |      0.0051 ns |      -6% |    1.0% |    1 |      - |         - |          NA |
| Helper3             | 3          | Information |  0.6513 ns | 0.0037 ns | 0.0035 ns |      0.0037 ns |      -4% |    1.1% |    2 |      - |         - |          NA |
| HelperRef3          | 3          | Information |  0.6186 ns | 0.0052 ns | 0.0048 ns |      0.0052 ns |      -9% |    0.9% |    1 |      - |         - |          NA |
| SourceGenerator3    | 3          | Information |  0.6765 ns | 0.0059 ns | 0.0056 ns |      0.0059 ns | baseline |         |    3 |      - |         - |          NA |
| SourceGeneratorRef3 | 3          | Information |  0.6273 ns | 0.0058 ns | 0.0055 ns |      0.0058 ns |      -7% |    1.1% |    1 |      - |         - |          NA |

### Mac - logging inside a loop
```
BenchmarkDotNet v0.13.12, macOS Sonoma 14.4.1 (23E224) [Darwin 23.4.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
  Job-SNSXTL : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD

Server=False  
```

| Method  | N          | Mean                | Error            | StdDev           | CI99.9% Margin    | Rank | Allocated |
|-------- |----------- |--------------------:|-----------------:|-----------------:|------------------:|-----:|----------:|
| Helper3 | 100        |            66.16 ns |         0.267 ns |         0.250 ns |         0.2672 ns |    1 |         - |
|         |            |                     |                  |                  |                   |      |           |
| Helper3 | 1000       |           679.16 ns |         3.029 ns |         2.685 ns |         3.0287 ns |    1 |         - |
|         |            |                     |                  |                  |                   |      |           |
| Helper3 | 1000000    |       669,615.53 ns |     3,117.873 ns |     2,916.460 ns |     3,117.8730 ns |    1 |       1 B |
|         |            |                     |                  |                  |                   |      |           |
| Helper3 | 1000000000 | 1,490,912,283.33 ns | 2,649,861.402 ns | 2,478,681.999 ns | 2,649,861.4018 ns |    1 |     736 B |
