# CharVsString

Compare call of the following methods with a char and string containing single character:
- `Contains` 
- `StartsWith` 
- `EndsWith` 
- `IndexOf` 
- `LastIndexOf`

The idea is to understand if the following Roslyn analyzers makes sense
- `CA1865`-`CA1867`: Use 'string.Method(char)' instead of 'string.Method(string)' for string with single char
- `CA1862`: Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
- `CA1847`: Use String.Contains(char) instead of String.Contains(string) with single characters

## Results

```
BenchmarkDotNet v0.13.12, macOS Sonoma 14.5 (23F79) [Darwin 23.5.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
  Job-TVIFKI : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD~~~~

Server=False
```

```

| Method                | N    | Mean          | Error      | StdDev    | Allocated |
|---------------------- |----- |--------------:|-----------:|----------:|----------:|
| ContainsChar          | 10   |     1.2535 ns |  0.0051 ns | 0.0048 ns |         - |
| ContainsStr           | 10   |     3.0381 ns |  0.0125 ns | 0.0104 ns |         - |
| StartsWithChar        | 10   |     0.0602 ns |  0.0024 ns | 0.0023 ns |         - |
| StartsWithStr         | 10   |     7.0909 ns |  0.0161 ns | 0.0135 ns |         - |
| EndsWithChar          | 10   |     0.1257 ns |  0.0026 ns | 0.0022 ns |         - |
| EndsWithStr           | 10   |     5.5942 ns |  0.0144 ns | 0.0127 ns |         - |
| IndexOfChar           | 10   |     1.2770 ns |  0.0020 ns | 0.0016 ns |         - |
| IndexOfStr            | 10   |    18.0973 ns |  0.1304 ns | 0.1156 ns |         - |
| IndexOfStrOrdinal     | 10   |     4.2271 ns |  0.0044 ns | 0.0036 ns |         - |
| LastIndexOfChar       | 10   |     1.2048 ns |  0.0038 ns | 0.0030 ns |         - |
| LastIndexOfStr        | 10   |    18.1586 ns |  0.0960 ns | 0.0802 ns |         - |
| LastIndexOfStrOrdinal | 10   |     6.2010 ns |  0.0196 ns | 0.0184 ns |         - |
|                       |      |               |            |           |           |
| ContainsChar          | 100  |     4.7988 ns |  0.0490 ns | 0.0458 ns |         - |
| ContainsStr           | 100  |     6.6255 ns |  0.0409 ns | 0.0341 ns |         - |
| StartsWithChar        | 100  |     0.0622 ns |  0.0046 ns | 0.0043 ns |         - |
| StartsWithStr         | 100  |     7.1192 ns |  0.0871 ns | 0.0814 ns |         - |
| EndsWithChar          | 100  |     0.1240 ns |  0.0015 ns | 0.0013 ns |         - |
| EndsWithStr           | 100  |     5.6428 ns |  0.0376 ns | 0.0352 ns |         - |
| IndexOfChar           | 100  |     4.8773 ns |  0.0530 ns | 0.0443 ns |         - |
| IndexOfStr            | 100  |   115.3874 ns |  0.6579 ns | 0.5493 ns |         - |
| IndexOfStrOrdinal     | 100  |     7.9615 ns |  0.0755 ns | 0.0631 ns |         - |
| LastIndexOfChar       | 100  |     5.3123 ns |  0.0475 ns | 0.0444 ns |         - |
| LastIndexOfStr        | 100  |   117.1172 ns |  1.3948 ns | 1.2365 ns |         - |
| LastIndexOfStrOrdinal | 100  |    10.4227 ns |  0.0722 ns | 0.0675 ns |         - |
|                       |      |               |            |           |           |
| ContainsChar          | 1000 |    51.1081 ns |  0.3333 ns | 0.3118 ns |         - |
| ContainsStr           | 1000 |    52.9295 ns |  0.4075 ns | 0.3812 ns |         - |
| StartsWithChar        | 1000 |     0.0487 ns |  0.0093 ns | 0.0087 ns |         - |
| StartsWithStr         | 1000 |     7.2211 ns |  0.0613 ns | 0.0544 ns |         - |
| EndsWithChar          | 1000 |     0.1092 ns |  0.0072 ns | 0.0068 ns |         - |
| EndsWithStr           | 1000 |     5.6382 ns |  0.0762 ns | 0.0713 ns |         - |
| IndexOfChar           | 1000 |    51.6625 ns |  0.7777 ns | 0.7275 ns |         - |
| IndexOfStr            | 1000 | 1,048.6731 ns | 10.2671 ns | 9.6039 ns |         - |
| IndexOfStrOrdinal     | 1000 |    53.2265 ns |  0.6253 ns | 0.5543 ns |         - |
| LastIndexOfChar       | 1000 |    53.3916 ns |  0.4452 ns | 0.4164 ns |         - |
| LastIndexOfStr        | 1000 | 1,033.7056 ns |  5.8900 ns | 5.5095 ns |         - |
| LastIndexOfStrOrdinal | 1000 |    57.6581 ns |  0.9045 ns | 0.8018 ns |         - |
```

## Analysis
It's clear that methods with `char` times faster comparing with string with single char.
For short strings (10 characters) the difference vary from 2 to 120 times, depends on method.
Interesting that using `StringComparison.Ordinal` is 3 times faster.
For larger strings the difference is that large, but absolute difference is impressive. 

```
| IndexOfChar           | 1000 |    51.6625 ns |  0.7777 ns | 0.7275 ns |         - |
| IndexOfStr            | 1000 | 1,048.6731 ns | 10.2671 ns | 9.6039 ns |         - |
```

The difference ~1,000 ns (1ms).

## Conclusions
These rules makes sense to follow (and easy to do).