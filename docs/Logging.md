# Logging benchmarks
Measuring different methods of logging to `ILogger`.

## Methodology
Created custom logger provider following [Microsoft recommendations](https://learn.microsoft.com/en-us/dotnet/core/extensions/custom-logging-provider).
The provider:
- does not store logging messages
- can call `formatter` - configurable by `UseFormatter` configuration property

Tested logging without, with one, two and three parameters.

There are 2 variations of tests with 3 parameters:
- `int`, `string` and `DateTime` (1 reference and 2 value types)
- all parameters of type `string` (reference type) 

## Tested logging methods

### Microsoft.Extensions.Logging
Probably the most commonly used method (`MEL`):
```csharp
    _logger.LogDebug(Message3, IntValue, StringValue, _startTime);
```

## Check if logging enabled
Using `MEL`, but first check if the logging is enabled
```csharp
    if (_logger.IsEnabled(LogLevel.Debug))
    {
        _logger.LogDebug(Message3, IntValue, StringValue, _startTime);
    }
```

## Using helper methods
Writing `if (_logger.IsEnabled(LogLevel.Debug))` is annoying, so I created helper methods.  
Using generics to avoid creation of `params object[]` if logging is disabled:
```csharp
    public static void Debug<T0, T1, T2>(this ILogger logger, [StructuredMessageTemplate] string messageTemplate, T0 p0, T1 p1, T2 p2)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.Log(LogLevel.Debug, messageTemplate, p0, p1, p2);
        }
    }
```

## Using [LoggerMessage]
Microsoft recommend using `[LoggerMessage]` for [High-performance logging in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/high-performance-logging).
```csharp
    [LoggerMessage(Level = LogLevel.Debug, Message = Message3)]
    partial void LoggerMessage3(int int1, string string1, DateTime time);
```

## Conclusions
  - `MEL` is the slowest method. It is slow (up to 17 times!) even when the logging is disabled
  - Surprisingly `SourceGenerator` is slower than `IsEnabledCheck` when logging is disabled. The difference is negligible (~1 ns), but still quite surprising
  - `SourceGenerator` is the fastest logging method in most cases when logging is enabled (except two cases)
    - `SourceGenerator` is about the same speed as any other method when message template has no arguments   
    - `SourceGenerator` is 79–100% faster when message template has one argument
    - `SourceGenerator` is 49–68% faster when message template has two arguments
    - `SourceGenerator` is 57–81% faster when message template has three arguments
  - When using formatter (more realistic scenario because formatter is used to produce a logging message)
    - `SourceGenerator` is ~3% **slower** comparing to `MEL` when message template has no arguments
    - `SourceGenerator` is 85–105% faster when message template has one argument
    - `SourceGenerator` is 36–43% faster when message template has two arguments
    - `SourceGenerator` is 42–51% faster when message template has three arguments

Please notice that despite a good relative performance, absolute values are not that impressive.  
We are talking about 5–60 ns (1 Nanosecond = 0.000000001 sec) difference.  
Put it in another way — to save 1 second CPU time, you need to log ~16,500,000–200,000,000 (16–200 million) messages. 

## Benchmark results

### Windows

```
BenchmarkDotNet v0.13.11, Windows 11 (10.0.22621.2861/22H2/2022Update/SunValley2)
Intel Core i7-10850H CPU 2.70GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-SOEYUO : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Server=False  
```
| Method              | Categories | UseFormatter | MinLogLevel | Mean       | Error     | StdDev     | Median     | CI99.9% Margin | Ratio    | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|---------------------|------------|--------------|------------ |-----------:|----------:|-----------:|-----------:|---------------:|---------:|--------:|-----:|-------:|----------:|------------:|
| **MEL0**            | **0**      | **False**    | **Debug**       |  **26.247 ns** | **0.1792 ns** |  **0.1496 ns** |  **26.199 ns** |      **0.1792 ns** |      **-8%** |    **1.7%** |    **1** |      **-** |         **-** |          **NA** |
| IsEnabledCheck0     | 0          | False        | Debug       |  31.558 ns | 0.6285 ns |  0.5248 ns |  31.510 ns |      0.6285 ns |     +11% |    2.3% |    4 |      - |         - |          NA |
| Helper0             | 0          | False        | Debug       |  30.769 ns | 0.3050 ns |  0.2704 ns |  30.803 ns |      0.3050 ns |      +8% |    1.4% |    3 |      - |         - |          NA |
| SourceGenerator0    | 0          | False        | Debug       |  28.489 ns | 0.3244 ns |  0.3735 ns |  28.463 ns |      0.3244 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL1                | 1          | False        | Debug       |  50.376 ns | 0.6529 ns |  0.6107 ns |  50.322 ns |      0.6529 ns |     +79% |    1.4% |    2 | 0.0089 |      56 B |          NA |
| IsEnabledCheck1     | 1          | False        | Debug       |  54.455 ns | 0.5836 ns |  0.5459 ns |  54.322 ns |      0.5836 ns |     +94% |    1.5% |    3 | 0.0089 |      56 B |          NA |
| Helper1             | 1          | False        | Debug       |  56.246 ns | 1.1126 ns |  1.3245 ns |  55.896 ns |      1.1126 ns |    +100% |    2.6% |    4 | 0.0089 |      56 B |          NA |
| SourceGenerator1    | 1          | False        | Debug       |  28.120 ns | 0.3507 ns |  0.3109 ns |  28.095 ns |      0.3507 ns | baseline |         |    1 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL2                | 2          | False        | Debug       |  52.257 ns | 0.5613 ns |  0.4976 ns |  52.163 ns |      0.5613 ns |     +49% |    1.2% |    2 | 0.0102 |      64 B |          NA |
| IsEnabledCheck2     | 2          | False        | Debug       |  58.434 ns | 1.1684 ns |  1.4349 ns |  58.116 ns |      1.1684 ns |     +66% |    3.2% |    3 | 0.0101 |      64 B |          NA |
| Helper2             | 2          | False        | Debug       |  59.150 ns | 0.9211 ns |  0.8616 ns |  59.476 ns |      0.9211 ns |     +68% |    1.9% |    3 | 0.0101 |      64 B |          NA |
| SourceGenerator2    | 2          | False        | Debug       |  35.125 ns | 0.2977 ns |  0.2785 ns |  35.115 ns |      0.2977 ns | baseline |         |    1 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL3                | 3          | False        | Debug       |  61.124 ns | 1.1778 ns |  2.7061 ns |  60.431 ns |      1.1778 ns |     +57% |    6.9% |    2 | 0.0153 |      96 B |          NA |
| MELRef3             | 3          | False        | Debug       |  59.658 ns | 1.2074 ns |  1.6926 ns |  59.490 ns |      1.2074 ns |     +59% |    3.9% |    2 | 0.0076 |      48 B |          NA |
| IsEnabledCheck3     | 3          | False        | Debug       |  66.448 ns | 0.8390 ns |  0.6551 ns |  66.706 ns |      0.8390 ns |     +81% |    2.7% |    4 | 0.0153 |      96 B |          NA |
| Helper3             | 3          | False        | Debug       |  71.546 ns | 1.4637 ns |  2.8892 ns |  71.605 ns |      1.4637 ns |     +93% |    4.7% |    5 | 0.0153 |      96 B |          NA |
| HelperRef3          | 3          | False        | Debug       |  64.223 ns | 1.3173 ns |  2.6002 ns |  63.685 ns |      1.3173 ns |     +78% |    4.1% |    3 | 0.0076 |      48 B |          NA |
| SourceGenerator3    | 3          | False        | Debug       |  37.256 ns | 0.7635 ns |  0.7499 ns |  37.076 ns |      0.7635 ns | baseline |         |    1 |      - |         - |          NA |
| SourceGeneratorRef3 | 3          | False        | Debug       |  37.399 ns | 0.7534 ns |  0.7399 ns |  37.221 ns |      0.7534 ns |      +0% |    3.1% |    1 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| **MEL0**            | **0**      | **False**    | **Information** |  **13.580 ns** | **0.1738 ns** |  **0.1541 ns** |  **13.513 ns** |      **0.1738 ns** |    **+518%** |    **5.9%** |    **4** |      **-** |         **-** |          **NA** |
| IsEnabledCheck0     | 0          | False        | Information |   1.800 ns | 0.0162 ns |  0.0144 ns |   1.799 ns |      0.0162 ns |     -18% |    5.6% |    1 |      - |         - |          NA |
| Helper0             | 0          | False        | Information |   2.238 ns | 0.0230 ns |  0.0215 ns |   2.228 ns |      0.0230 ns |      +2% |    5.3% |    3 |      - |         - |          NA |
| SourceGenerator0    | 0          | False        | Information |   2.116 ns | 0.0705 ns |  0.1178 ns |   2.113 ns |      0.0705 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL1                | 1          | False        | Information |  35.570 ns | 0.6401 ns |  0.5674 ns |  35.458 ns |      0.6401 ns |  +1,171% |    2.0% |    4 | 0.0089 |      56 B |          NA |
| IsEnabledCheck1     | 1          | False        | Information |   1.817 ns | 0.0243 ns |  0.0215 ns |   1.810 ns |      0.0243 ns |     -35% |    1.9% |    1 |      - |         - |          NA |
| Helper1             | 1          | False        | Information |   2.417 ns | 0.0486 ns |  0.0431 ns |   2.416 ns |      0.0486 ns |     -14% |    2.1% |    2 |      - |         - |          NA |
| SourceGenerator1    | 1          | False        | Information |   2.799 ns | 0.0446 ns |  0.0417 ns |   2.788 ns |      0.0446 ns | baseline |         |    3 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL2                | 2          | False        | Information |  37.595 ns | 0.7139 ns |  0.6329 ns |  37.295 ns |      0.7139 ns |  +1,574% |    2.1% |    4 | 0.0102 |      64 B |          NA |
| IsEnabledCheck2     | 2          | False        | Information |   1.777 ns | 0.0238 ns |  0.0198 ns |   1.775 ns |      0.0238 ns |     -21% |    2.1% |    1 |      - |         - |          NA |
| Helper2             | 2          | False        | Information |   2.379 ns | 0.0195 ns |  0.0173 ns |   2.380 ns |      0.0195 ns |      +6% |    1.9% |    3 |      - |         - |          NA |
| SourceGenerator2    | 2          | False        | Information |   2.246 ns | 0.0384 ns |  0.0341 ns |   2.243 ns |      0.0384 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL3                | 3          | False        | Information |  60.211 ns | 1.2351 ns |  3.4223 ns |  60.686 ns |      1.2351 ns |  +2,213% |    5.1% |    4 | 0.0153 |      96 B |          NA |
| MELRef3             | 3          | False        | Information |  41.257 ns | 0.8165 ns |  1.6680 ns |  40.834 ns |      0.8165 ns |  +1,616% |    3.4% |    5 | 0.0076 |      48 B |          NA |
| IsEnabledCheck3     | 3          | False        | Information |   1.838 ns | 0.0573 ns |  0.0536 ns |   1.841 ns |      0.0573 ns |     -29% |    3.5% |    1 |      - |         - |          NA |
| Helper3             | 3          | False        | Information |   2.631 ns | 0.0491 ns |  0.0459 ns |   2.641 ns |      0.0491 ns |      +5% |    2.7% |    4 |      - |         - |          NA |
| HelperRef3          | 3          | False        | Information |   2.159 ns | 0.0630 ns |  0.0559 ns |   2.149 ns |      0.0630 ns |     -14% |    2.2% |    1 |      - |         - |          NA |
| SourceGenerator3    | 3          | False        | Information |   2.496 ns | 0.0367 ns |  0.0307 ns |   2.495 ns |      0.0367 ns | baseline |         |    3 |      - |         - |          NA |
| SourceGeneratorRef3 | 3          | False        | Information |   2.408 ns | 0.0749 ns |  0.0735 ns |   2.405 ns |      0.0749 ns |      -3% |    3.9% |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| **MEL0**            | **0**      | **True**     | **Debug**       |  **27.368 ns** | **0.2365 ns** |  **0.2096 ns** |  **27.309 ns** |      **0.2365 ns** |      **-3%** |    **1.6%** |    **1** |      **-** |         **-** |          **NA** |
| IsEnabledCheck0     | 0          | True         | Debug       |  33.601 ns | 0.3107 ns |  0.2594 ns |  33.594 ns |      0.3107 ns |     +19% |    1.9% |    3 |      - |         - |          NA |
| Helper0             | 0          | True         | Debug       |  33.653 ns | 0.4608 ns |  0.4085 ns |  33.533 ns |      0.4608 ns |     +19% |    1.6% |    3 |      - |         - |          NA |
| SourceGenerator0    | 0          | True         | Debug       |  28.249 ns | 0.5040 ns |  0.4209 ns |  28.128 ns |      0.5040 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL1                | 1          | True         | Debug       | 117.008 ns | 1.4670 ns |  1.3005 ns | 116.408 ns |      1.4670 ns |     +85% |    1.2% |    2 | 0.0191 |     120 B |        +88% |
| IsEnabledCheck1     | 1          | True         | Debug       | 125.759 ns | 1.5587 ns |  1.3818 ns | 126.006 ns |      1.5587 ns |     +99% |    1.8% |    3 | 0.0191 |     120 B |        +88% |
| Helper1             | 1          | True         | Debug       | 129.149 ns | 2.4180 ns |  3.5443 ns | 127.814 ns |      2.4180 ns |    +105% |    3.7% |    4 | 0.0191 |     120 B |        +88% |
| SourceGenerator1    | 1          | True         | Debug       |  63.226 ns | 1.0794 ns |  0.9014 ns |  62.890 ns |      1.0794 ns | baseline |         |    1 | 0.0101 |      64 B |             |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL2                | 2          | True         | Debug       | 148.522 ns | 2.9496 ns |  4.6784 ns | 146.684 ns |      2.9496 ns |     +36% |    4.1% |    2 | 0.0267 |     168 B |        +62% |
| IsEnabledCheck2     | 2          | True         | Debug       | 156.628 ns | 0.9991 ns |  0.8343 ns | 156.483 ns |      0.9991 ns |     +43% |    2.3% |    3 | 0.0267 |     168 B |        +62% |
| Helper2             | 2          | True         | Debug       | 155.036 ns | 1.7029 ns |  1.5929 ns | 155.233 ns |      1.7029 ns |     +42% |    2.7% |    3 | 0.0267 |     168 B |        +62% |
| SourceGenerator2    | 2          | True         | Debug       | 110.046 ns | 1.8817 ns |  2.6987 ns | 109.397 ns |      1.8817 ns | baseline |         |    1 | 0.0166 |     104 B |             |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL3                | 3          | True         | Debug       | 238.535 ns | 4.6468 ns | 13.2575 ns | 234.519 ns |      4.6468 ns |     +51% |    7.1% |    3 | 0.0381 |     240 B |        +67% |
| MELRef3             | 3          | True         | Debug       | 159.638 ns | 3.1891 ns |  5.8315 ns | 158.494 ns |      3.1891 ns |      -8% |    5.2% |    2 | 0.0317 |     200 B |        +39% |
| IsEnabledCheck3     | 3          | True         | Debug       | 236.118 ns | 4.4925 ns |  9.7663 ns | 234.960 ns |      4.4925 ns |     +51% |    6.9% |    3 | 0.0381 |     240 B |        +67% |
| Helper3             | 3          | True         | Debug       | 248.223 ns | 5.5266 ns | 16.2954 ns | 245.653 ns |      5.5266 ns |     +41% |    8.0% |    4 | 0.0381 |     240 B |        +67% |
| HelperRef3          | 3          | True         | Debug       | 173.778 ns | 3.5114 ns |  9.1267 ns | 171.296 ns |      3.5114 ns |      +1% |    6.4% |    3 | 0.0317 |     200 B |        +39% |
| SourceGenerator3    | 3          | True         | Debug       | 174.994 ns | 3.5015 ns |  7.4621 ns | 174.061 ns |      3.5015 ns | baseline |         |    3 | 0.0229 |     144 B |             |
| SourceGeneratorRef3 | 3          | True         | Debug       | 151.039 ns | 2.9890 ns |  8.3322 ns | 148.403 ns |      2.9890 ns |     -12% |    7.5% |    1 | 0.0241 |     152 B |         +6% |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| **MEL0**            | **0**      | **True**     | **Information** |  **13.548 ns** | **0.0935 ns** |  **0.0780 ns** |  **13.526 ns** |      **0.0935 ns** |    **+559%** |    **4.1%** |    **4** |      **-** |         **-** |          **NA** |
| IsEnabledCheck0     | 0          | True         | Information |   1.925 ns | 0.0372 ns |  0.0330 ns |   1.920 ns |      0.0372 ns |      -6% |    4.7% |    1 |      - |         - |          NA |
| Helper0             | 0          | True         | Information |   2.307 ns | 0.0434 ns |  0.0406 ns |   2.300 ns |      0.0434 ns |     +12% |    4.5% |    3 |      - |         - |          NA |
| SourceGenerator0    | 0          | True         | Information |   2.083 ns | 0.0704 ns |  0.0864 ns |   2.075 ns |      0.0704 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL1                | 1          | True         | Information |  37.269 ns | 0.7529 ns |  0.9790 ns |  37.277 ns |      0.7529 ns |  +1,434% |    5.5% |    4 | 0.0089 |      56 B |          NA |
| IsEnabledCheck1     | 1          | True         | Information |   1.785 ns | 0.0187 ns |  0.0156 ns |   1.787 ns |      0.0187 ns |     -26% |    4.1% |    1 |      - |         - |          NA |
| Helper1             | 1          | True         | Information |   2.489 ns | 0.0780 ns |  0.0867 ns |   2.464 ns |      0.0780 ns |      +2% |    6.1% |    3 |      - |         - |          NA |
| SourceGenerator1    | 1          | True         | Information |   2.414 ns | 0.0764 ns |  0.1120 ns |   2.385 ns |      0.0764 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL2                | 2          | True         | Information |  38.066 ns | 0.6263 ns |  0.5552 ns |  37.889 ns |      0.6263 ns |  +1,594% |    1.8% |    4 | 0.0102 |      64 B |          NA |
| IsEnabledCheck2     | 2          | True         | Information |   1.838 ns | 0.0645 ns |  0.0768 ns |   1.819 ns |      0.0645 ns |     -18% |    4.6% |    1 |      - |         - |          NA |
| Helper2             | 2          | True         | Information |   2.453 ns | 0.0778 ns |  0.0865 ns |   2.424 ns |      0.0778 ns |      +9% |    4.1% |    3 |      - |         - |          NA |
| SourceGenerator2    | 2          | True         | Information |   2.245 ns | 0.0318 ns |  0.0297 ns |   2.237 ns |      0.0318 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |            |            |                |          |         |      |        |           |             |
| MEL3                | 3          | True         | Information |  47.772 ns | 0.9906 ns |  2.6612 ns |  47.140 ns |      0.9906 ns |  +1,748% |    8.0% |    3 | 0.0153 |      96 B |          NA |
| IsEnabledCheck3     | 3          | True         | Information |   1.793 ns | 0.0411 ns |  0.0321 ns |   1.801 ns |      0.0411 ns |     -31% |    5.0% |    1 |      - |         - |          NA |
| MELRef3             | 3          | True         | Information |  40.078 ns | 0.6445 ns |  0.5713 ns |  40.043 ns |      0.6445 ns |  +1,478% |    2.9% |    4 | 0.0076 |      48 B |          NA |
| Helper3             | 3          | True         | Information |   2.641 ns | 0.0291 ns |  0.0243 ns |   2.639 ns |      0.0291 ns |      +4% |    2.5% |    3 |      - |         - |          NA |
| HelperRef3          | 3          | True         | Information |   2.243 ns | 0.0206 ns |  0.0160 ns |   2.244 ns |      0.0206 ns |     -12% |    2.6% |    1 |      - |         - |          NA |
| SourceGenerator3    | 3          | True         | Information |   2.542 ns | 0.0660 ns |  0.0585 ns |   2.563 ns |      0.0660 ns | baseline |         |    2 |      - |         - |          NA |
| SourceGeneratorRef3 | 3          | True         | Information |   2.256 ns | 0.0304 ns |  0.0269 ns |   2.259 ns |      0.0304 ns |     -11% |    2.7% |    1 |      - |         - |          NA |

### Mac
```
BenchmarkDotNet v0.13.12, macOS Sonoma 14.4 (23E214) [Darwin 23.4.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 8.0.201
[Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
Job-WCTOJS : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD

Server=False
```

| Method              | Categories | UseFormatter | MinLogLevel | Mean       | Error     | StdDev    | CI99.9% Margin | Ratio    | RatioSD | Rank | Gen0   | Allocated | Alloc Ratio |
|-------------------- |----------- |------------- |------------ |-----------:|----------:|----------:|---------------:|---------:|--------:|-----:|-------:|----------:|------------:|
| MEL0                | 0          | False        | Debug       |  13.918 ns | 0.1117 ns | 0.1045 ns |      0.1117 ns |      -8% |    1.2% |    1 |      - |         - |          NA |
| IsEnabledCheck0     | 0          | False        | Debug       |  17.566 ns | 0.1017 ns | 0.0951 ns |      0.1017 ns |     +16% |    0.9% |    3 |      - |         - |          NA |
| Helper0             | 0          | False        | Debug       |  18.053 ns | 0.1430 ns | 0.1268 ns |      0.1430 ns |     +19% |    0.9% |    4 |      - |         - |          NA |
| SourceGenerator0    | 0          | False        | Debug       |  15.124 ns | 0.1136 ns | 0.1007 ns |      0.1136 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL1                | 1          | False        | Debug       |  30.685 ns | 0.3128 ns | 0.2926 ns |      0.3128 ns |    +106% |    1.2% |    2 | 0.0067 |      56 B |          NA |
| IsEnabledCheck1     | 1          | False        | Debug       |  34.757 ns | 0.1481 ns | 0.1385 ns |      0.1481 ns |    +133% |    0.6% |    4 | 0.0067 |      56 B |          NA |
| Helper1             | 1          | False        | Debug       |  34.217 ns | 0.2687 ns | 0.2514 ns |      0.2687 ns |    +130% |    1.0% |    3 | 0.0067 |      56 B |          NA |
| SourceGenerator1    | 1          | False        | Debug       |  14.891 ns | 0.0875 ns | 0.0819 ns |      0.0875 ns | baseline |         |    1 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL2                | 2          | False        | Debug       |  31.668 ns | 0.1907 ns | 0.1592 ns |      0.1907 ns |     +66% |    0.6% |    2 | 0.0076 |      64 B |          NA |
| IsEnabledCheck2     | 2          | False        | Debug       |  34.899 ns | 0.1844 ns | 0.1725 ns |      0.1844 ns |     +83% |    0.8% |    3 | 0.0076 |      64 B |          NA |
| Helper2             | 2          | False        | Debug       |  35.353 ns | 0.1267 ns | 0.0989 ns |      0.1267 ns |     +85% |    0.5% |    3 | 0.0076 |      64 B |          NA |
| SourceGenerator2    | 2          | False        | Debug       |  19.095 ns | 0.0929 ns | 0.0823 ns |      0.0929 ns | baseline |         |    1 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL3                | 3          | False        | Debug       |  37.271 ns | 0.1577 ns | 0.1476 ns |      0.1577 ns |     +91% |    0.6% |    5 | 0.0114 |      96 B |          NA |
| MELRef3             | 3          | False        | Debug       |  33.904 ns | 0.1130 ns | 0.1057 ns |      0.1130 ns |     +74% |    0.5% |    3 | 0.0057 |      48 B |          NA |
| IsEnabledCheck3     | 3          | False        | Debug       |  39.060 ns | 0.0905 ns | 0.0756 ns |      0.0905 ns |    +100% |    0.5% |    6 | 0.0114 |      96 B |          NA |
| Helper3             | 3          | False        | Debug       |  39.724 ns | 0.1058 ns | 0.0938 ns |      0.1058 ns |    +104% |    0.5% |    7 | 0.0114 |      96 B |          NA |
| HelperRef3          | 3          | False        | Debug       |  36.539 ns | 0.2806 ns | 0.2625 ns |      0.2806 ns |     +87% |    0.8% |    4 | 0.0057 |      48 B |          NA |
| SourceGenerator3    | 3          | False        | Debug       |  19.490 ns | 0.0973 ns | 0.0863 ns |      0.0973 ns | baseline |         |    2 |      - |         - |          NA |
| SourceGeneratorRef3 | 3          | False        | Debug       |  19.183 ns | 0.1368 ns | 0.1280 ns |      0.1368 ns |      -2% |    0.9% |    1 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL0                | 0          | False        | Information |   7.941 ns | 0.0532 ns | 0.0472 ns |      0.0532 ns |    +372% |    1.5% |    4 |      - |         - |          NA |
| IsEnabledCheck0     | 0          | False        | Information |   1.265 ns | 0.0239 ns | 0.0224 ns |      0.0239 ns |     -25% |    2.5% |    1 |      - |         - |          NA |
| Helper0             | 0          | False        | Information |   1.761 ns | 0.0188 ns | 0.0175 ns |      0.0188 ns |      +4% |    1.3% |    3 |      - |         - |          NA |
| SourceGenerator0    | 0          | False        | Information |   1.685 ns | 0.0187 ns | 0.0175 ns |      0.0187 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL1                | 1          | False        | Information |  24.273 ns | 0.1692 ns | 0.1500 ns |      0.1692 ns |  +1,286% |    1.5% |    3 | 0.0067 |      56 B |          NA |
| IsEnabledCheck1     | 1          | False        | Information |   1.300 ns | 0.0178 ns | 0.0167 ns |      0.0178 ns |     -26% |    1.6% |    1 |      - |         - |          NA |
| Helper1             | 1          | False        | Information |   1.760 ns | 0.0051 ns | 0.0048 ns |      0.0051 ns |      +1% |    1.2% |    2 |      - |         - |          NA |
| SourceGenerator1    | 1          | False        | Information |   1.751 ns | 0.0235 ns | 0.0209 ns |      0.0235 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL2                | 2          | False        | Information |  28.454 ns | 0.1814 ns | 0.1416 ns |      0.1814 ns |  +1,562% |    0.4% |    4 | 0.0076 |      64 B |          NA |
| IsEnabledCheck2     | 2          | False        | Information |   1.268 ns | 0.0099 ns | 0.0092 ns |      0.0099 ns |     -26% |    0.7% |    1 |      - |         - |          NA |
| Helper2             | 2          | False        | Information |   1.809 ns | 0.0264 ns | 0.0247 ns |      0.0264 ns |      +6% |    1.4% |    3 |      - |         - |          NA |
| SourceGenerator2    | 2          | False        | Information |   1.713 ns | 0.0033 ns | 0.0031 ns |      0.0033 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL3                | 3          | False        | Information |  30.251 ns | 0.1129 ns | 0.1056 ns |      0.1129 ns |  +1,558% |    1.2% |    7 | 0.0114 |      96 B |          NA |
| MELRef3             | 3          | False        | Information |  27.733 ns | 0.0934 ns | 0.0729 ns |      0.0934 ns |  +1,419% |    1.0% |    6 | 0.0057 |      48 B |          NA |
| IsEnabledCheck3     | 3          | False        | Information |   1.293 ns | 0.0188 ns | 0.0175 ns |      0.0188 ns |     -29% |    1.6% |    1 |      - |         - |          NA |
| Helper3             | 3          | False        | Information |   1.964 ns | 0.0208 ns | 0.0194 ns |      0.0208 ns |      +8% |    1.2% |    5 |      - |         - |          NA |
| HelperRef3          | 3          | False        | Information |   1.748 ns | 0.0262 ns | 0.0245 ns |      0.0262 ns |      -4% |    1.8% |    2 |      - |         - |          NA |
| SourceGenerator3    | 3          | False        | Information |   1.825 ns | 0.0189 ns | 0.0177 ns |      0.0189 ns | baseline |         |    4 |      - |         - |          NA |
| SourceGeneratorRef3 | 3          | False        | Information |   1.777 ns | 0.0072 ns | 0.0064 ns |      0.0072 ns |      -3% |    1.0% |    3 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL0                | 0          | True         | Debug       |  15.959 ns | 0.1013 ns | 0.0947 ns |      0.1013 ns |      +4% |    0.6% |    2 |      - |         - |          NA |
| IsEnabledCheck0     | 0          | True         | Debug       |  19.073 ns | 0.1118 ns | 0.1046 ns |      0.1118 ns |     +25% |    0.8% |    3 |      - |         - |          NA |
| Helper0             | 0          | True         | Debug       |  19.262 ns | 0.1996 ns | 0.1867 ns |      0.1996 ns |     +26% |    1.2% |    3 |      - |         - |          NA |
| SourceGenerator0    | 0          | True         | Debug       |  15.280 ns | 0.0689 ns | 0.0645 ns |      0.0689 ns | baseline |         |    1 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL1                | 1          | True         | Debug       |  70.674 ns | 0.2221 ns | 0.1969 ns |      0.2221 ns |     +86% |    0.7% |    2 | 0.0143 |     120 B |        +88% |
| IsEnabledCheck1     | 1          | True         | Debug       |  72.973 ns | 0.6253 ns | 0.5849 ns |      0.6253 ns |     +92% |    0.8% |    3 | 0.0143 |     120 B |        +88% |
| Helper1             | 1          | True         | Debug       |  73.541 ns | 0.7103 ns | 0.6644 ns |      0.7103 ns |     +93% |    1.0% |    3 | 0.0143 |     120 B |        +88% |
| SourceGenerator1    | 1          | True         | Debug       |  38.020 ns | 0.2416 ns | 0.2260 ns |      0.2416 ns | baseline |         |    1 | 0.0076 |      64 B |             |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL2                | 2          | True         | Debug       |  92.148 ns | 0.9584 ns | 0.8496 ns |      0.9584 ns |     +49% |    1.1% |    2 | 0.0200 |     168 B |        +62% |
| IsEnabledCheck2     | 2          | True         | Debug       |  94.593 ns | 0.7767 ns | 0.7266 ns |      0.7767 ns |     +53% |    0.9% |    3 | 0.0200 |     168 B |        +62% |
| Helper2             | 2          | True         | Debug       |  95.142 ns | 0.3685 ns | 0.3267 ns |      0.3685 ns |     +54% |    0.7% |    3 | 0.0200 |     168 B |        +62% |
| SourceGenerator2    | 2          | True         | Debug       |  61.741 ns | 0.3836 ns | 0.3588 ns |      0.3836 ns | baseline |         |    1 | 0.0124 |     104 B |             |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL3                | 3          | True         | Debug       | 131.334 ns | 0.7951 ns | 0.7048 ns |      0.7951 ns |     +60% |    0.8% |    4 | 0.0286 |     240 B |        +67% |
| MELRef3             | 3          | True         | Debug       | 100.544 ns | 0.8496 ns | 0.7531 ns |      0.8496 ns |     +23% |    0.8% |    3 | 0.0238 |     200 B |        +39% |
| IsEnabledCheck3     | 3          | True         | Debug       | 138.891 ns | 0.8255 ns | 0.7722 ns |      0.8255 ns |     +70% |    0.5% |    6 | 0.0286 |     240 B |        +67% |
| Helper3             | 3          | True         | Debug       | 136.839 ns | 0.6875 ns | 0.5741 ns |      0.6875 ns |     +67% |    0.8% |    5 | 0.0286 |     240 B |        +67% |
| HelperRef3          | 3          | True         | Debug       | 101.775 ns | 0.4579 ns | 0.3824 ns |      0.4579 ns |     +24% |    0.8% |    3 | 0.0238 |     200 B |        +39% |
| SourceGenerator3    | 3          | True         | Debug       |  81.865 ns | 0.4164 ns | 0.3895 ns |      0.4164 ns | baseline |         |    1 | 0.0172 |     144 B |             |
| SourceGeneratorRef3 | 3          | True         | Debug       |  83.037 ns | 0.3694 ns | 0.3456 ns |      0.3694 ns |      +1% |    0.6% |    2 | 0.0181 |     152 B |         +6% |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL0                | 0          | True         | Information |   7.813 ns | 0.1035 ns | 0.0968 ns |      0.1035 ns |    +360% |    1.5% |    4 |      - |         - |          NA |
| IsEnabledCheck0     | 0          | True         | Information |   1.282 ns | 0.0172 ns | 0.0161 ns |      0.0172 ns |     -25% |    1.4% |    1 |      - |         - |          NA |
| Helper0             | 0          | True         | Information |   1.728 ns | 0.0158 ns | 0.0148 ns |      0.0158 ns |      +2% |    1.1% |    3 |      - |         - |          NA |
| SourceGenerator0    | 0          | True         | Information |   1.696 ns | 0.0161 ns | 0.0142 ns |      0.0161 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL1                | 1          | True         | Information |  24.010 ns | 0.0973 ns | 0.0813 ns |      0.0973 ns |  +1,282% |    1.7% |    4 | 0.0067 |      56 B |          NA |
| IsEnabledCheck1     | 1          | True         | Information |   1.277 ns | 0.0283 ns | 0.0265 ns |      0.0283 ns |     -27% |    2.1% |    1 |      - |         - |          NA |
| Helper1             | 1          | True         | Information |   1.801 ns | 0.0252 ns | 0.0236 ns |      0.0252 ns |      +3% |    2.1% |    3 |      - |         - |          NA |
| SourceGenerator1    | 1          | True         | Information |   1.742 ns | 0.0316 ns | 0.0295 ns |      0.0316 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL2                | 2          | True         | Information |  26.101 ns | 0.2558 ns | 0.2392 ns |      0.2558 ns |  +1,404% |    1.3% |    4 | 0.0076 |      64 B |          NA |
| IsEnabledCheck2     | 2          | True         | Information |   1.347 ns | 0.0106 ns | 0.0099 ns |      0.0106 ns |     -22% |    0.8% |    1 |      - |         - |          NA |
| Helper2             | 2          | True         | Information |   1.790 ns | 0.0146 ns | 0.0137 ns |      0.0146 ns |      +3% |    0.7% |    3 |      - |         - |          NA |
| SourceGenerator2    | 2          | True         | Information |   1.736 ns | 0.0152 ns | 0.0142 ns |      0.0152 ns | baseline |         |    2 |      - |         - |          NA |
|                     |            |              |             |            |           |           |                |          |         |      |        |           |             |
| MEL3                | 3          | True         | Information |  30.523 ns | 0.1735 ns | 0.1623 ns |      0.1735 ns |  +1,580% |    0.8% |    7 | 0.0114 |      96 B |          NA |
| MELRef3             | 3          | True         | Information |  27.761 ns | 0.0540 ns | 0.0505 ns |      0.0540 ns |  +1,428% |    0.5% |    6 | 0.0057 |      48 B |          NA |
| IsEnabledCheck3     | 3          | True         | Information |   1.346 ns | 0.0062 ns | 0.0058 ns |      0.0062 ns |     -26% |    0.6% |    1 |      - |         - |          NA |
| Helper3             | 3          | True         | Information |   1.932 ns | 0.0092 ns | 0.0086 ns |      0.0092 ns |      +6% |    0.8% |    5 |      - |         - |          NA |
| HelperRef3          | 3          | True         | Information |   1.732 ns | 0.0132 ns | 0.0124 ns |      0.0132 ns |      -5% |    0.8% |    2 |      - |         - |          NA |
| SourceGenerator3    | 3          | True         | Information |   1.817 ns | 0.0100 ns | 0.0094 ns |      0.0100 ns | baseline |         |    4 |      - |         - |          NA |
| SourceGeneratorRef3 | 3          | True         | Information |   1.769 ns | 0.0062 ns | 0.0055 ns |      0.0062 ns |      -3% |    0.7% |    3 |      - |         - |          NA |

