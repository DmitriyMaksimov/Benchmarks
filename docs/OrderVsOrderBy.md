# OrderVsOrderBy

Compare LINQ `Order()` and `OrderBy(x => x)` as per `Meziantou.Analyzer`'s rule [MA0159 "Use 'Order' instead of 'OrderBy'"](https://github.com/meziantou/Meziantou.Analyzer/blob/main/docs/Rules/MA0159.md)

## Results

```
BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 9.0.100-rc.2.24474.11
  [Host]   : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
  .NET 8.0 : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD

Job=.NET 8.0  Runtime=.NET 8.0  Server=False  
```

```
| Method  | N     | InitializationOrder | Mean          | Ratio    | Gen0    | Gen1   | Allocated | Alloc Ratio |
|-------- |------ |-------------------- |--------------:|---------:|--------:|-------:|----------:|------------:|
| OrderBy | 10    | Ascending           |      74.96 ns |    +148% |  0.0554 |      - |     464 B |       +262% |
| Order   | 10    | Ascending           |      30.18 ns | baseline |  0.0153 |      - |     128 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 10    | Descending          |     128.64 ns |    +145% |  0.0553 |      - |     464 B |       +262% |
| Order   | 10    | Descending          |      52.60 ns | baseline |  0.0153 |      - |     128 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 10    | Random              |     107.21 ns |    +163% |  0.0554 |      - |     464 B |       +262% |
| Order   | 10    | Random              |      40.75 ns | baseline |  0.0153 |      - |     128 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 100   | Ascending           |     737.77 ns |    +254% |  0.2270 |      - |    1904 B |       +290% |
| Order   | 100   | Ascending           |     208.52 ns | baseline |  0.0582 |      - |     488 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 100   | Descending          |   1,062.99 ns |    +236% |  0.2270 |      - |    1904 B |       +290% |
| Order   | 100   | Descending          |     316.41 ns | baseline |  0.0582 |      - |     488 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 100   | Random              |   1,332.66 ns |    +216% |  0.2270 |      - |    1904 B |       +290% |
| Order   | 100   | Random              |     422.16 ns | baseline |  0.0582 |      - |     488 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 1000  | Ascending           |  11,090.64 ns |    +253% |  1.9379 |      - |   16304 B |       +299% |
| Order   | 1000  | Ascending           |   3,139.06 ns | baseline |  0.4883 |      - |    4088 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 1000  | Descending          |  18,598.07 ns |    +261% |  1.9226 |      - |   16304 B |       +299% |
| Order   | 1000  | Descending          |   5,146.47 ns | baseline |  0.4883 |      - |    4088 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 1000  | Random              |  23,217.45 ns |    +293% |  1.9226 |      - |   16304 B |       +299% |
| Order   | 1000  | Random              |   5,917.49 ns | baseline |  0.4883 |      - |    4088 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 10000 | Ascending           | 164,897.03 ns |    +256% | 19.0430 | 0.7324 |  160304 B |       +300% |
| Order   | 10000 | Ascending           |  46,330.27 ns | baseline |  4.7607 |      - |   40088 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 10000 | Descending          | 273,476.29 ns |    +259% | 19.0430 | 0.4883 |  160304 B |       +300% |
| Order   | 10000 | Descending          |  76,180.40 ns | baseline |  4.7607 |      - |   40088 B |             |
|         |       |                     |               |          |         |        |           |             |
| OrderBy | 10000 | Random              | 651,616.27 ns |    +109% | 18.5547 | 0.9766 |  160305 B |       +300% |
| Order   | 10000 | Random              | 312,170.39 ns | baseline |  4.3945 |      - |   40088 B |             |
```

## Analysis
`Order` is significantly (2-4 times) faster then `OrderBy` and allocates 3-4 times less memory.  
Interesting observation - sorting time depends on initial order of elements:
- When elements are already in sorted order `Order/OrderBy` is the fastest
- When elements are in reverse sorted order `Order/OrderBy` comes second (~1.44 times)
- Sorting a random elements is the slowest (~1.8 times comparing to leader) (except case with 10 elements)

```
| OrderBy | 100   | Ascending           |     737.77 ns |    +254% |  0.2270 |      - |    1904 B |       +290% |
| OrderBy | 100   | Descending          |   1,062.99 ns |    +236% |  0.2270 |      - |    1904 B |       +290% |
| OrderBy | 100   | Random              |   1,332.66 ns |    +216% |  0.2270 |      - |    1904 B |       +290% |
```

## Conclusions
The rule makes sense to follow and easy to do.
