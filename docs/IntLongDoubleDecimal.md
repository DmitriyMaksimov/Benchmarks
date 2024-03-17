# Numeric types
Compare performance of the following operations of `int`, `long`, `double` and `decimal` numeric types
- `Parse`
- `TryParse`
- `ToString`
- Arithmetic operations

## Results
```
BenchmarkDotNet v0.13.12, macOS Sonoma 14.4 (23E214) [Darwin 23.4.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 8.0.201
[Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
Job-LOZAYS : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD

Server=False
```

| Method                | Categories     | N    | Mean          | Error       | StdDev      | CI99.9% Margin | Ratio | RatioSD | Rank | Gen0   | Gen1   | Allocated | Alloc Ratio |
|---------------------- |--------------- |----- |--------------:|------------:|------------:|---------------:|------:|--------:|-----:|-------:|-------:|----------:|------------:|
| IntMultiplication     | Multiplication | 10   |      5.246 ns |   0.0338 ns |   0.0264 ns |      0.0338 ns |  1.00 |    0.00 |    2 |      - |      - |         - |          NA |
| LongMultiplication    | Multiplication | 10   |      5.245 ns |   0.0401 ns |   0.0375 ns |      0.0401 ns |  1.00 |    0.01 |    2 |      - |      - |         - |          NA |
| DoubleMultiplication  | Multiplication | 10   |      5.109 ns |   0.0408 ns |   0.0340 ns |      0.0408 ns |  0.97 |    0.01 |    1 |      - |      - |         - |          NA |
| DecimalMultiplication | Multiplication | 10   |     27.643 ns |   0.2378 ns |   0.2108 ns |      0.2378 ns |  5.28 |    0.04 |    3 |      - |      - |         - |          NA |
|                       |                |      |               |             |             |                |       |         |      |        |        |           |             |
| IntParse              | Parse          | 10   |     91.499 ns |   0.8733 ns |   0.7741 ns |      0.8733 ns |  1.00 |    0.00 |    1 |      - |      - |         - |          NA |
| LongParse             | Parse          | 10   |     98.588 ns |   1.3241 ns |   1.2385 ns |      1.3241 ns |  1.08 |    0.02 |    2 |      - |      - |         - |          NA |
| DoubleParse           | Parse          | 10   |    387.164 ns |   4.6685 ns |   4.3670 ns |      4.6685 ns |  4.23 |    0.06 |    3 |      - |      - |         - |          NA |
| DecimalParse          | Parse          | 10   |    502.127 ns |   6.6300 ns |   6.2017 ns |      6.6300 ns |  5.48 |    0.08 |    5 |      - |      - |         - |          NA |
| IntTryParse           | Parse          | 10   |     90.324 ns |   1.2012 ns |   1.1236 ns |      1.2012 ns |  0.99 |    0.01 |    1 |      - |      - |         - |          NA |
| LongTryParse          | Parse          | 10   |     98.599 ns |   0.9987 ns |   0.8853 ns |      0.9987 ns |  1.08 |    0.01 |    2 |      - |      - |         - |          NA |
| DoubleTryParse        | Parse          | 10   |    385.018 ns |   1.8188 ns |   1.7013 ns |      1.8188 ns |  4.21 |    0.04 |    3 |      - |      - |         - |          NA |
| DecimalTryParse       | Parse          | 10   |    489.391 ns |   1.7300 ns |   1.5336 ns |      1.7300 ns |  5.35 |    0.05 |    4 |      - |      - |         - |          NA |
|                       |                |      |               |             |             |                |       |         |      |        |        |           |             |
| LongSum               | Sum            | 10   |      4.104 ns |   0.0108 ns |   0.0084 ns |      0.0108 ns |  1.00 |    0.00 |    1 |      - |      - |         - |          NA |
| DoubleSum             | Sum            | 10   |      4.118 ns |   0.0218 ns |   0.0193 ns |      0.0218 ns |  1.00 |    0.01 |    1 |      - |      - |         - |          NA |
| DecimalSum            | Sum            | 10   |     46.579 ns |   0.3262 ns |   0.3051 ns |      0.3262 ns | 11.33 |    0.06 |    2 |      - |      - |         - |          NA |
|                       |                |      |               |             |             |                |       |         |      |        |        |           |             |
| IntToString           | ToString       | 10   |     82.019 ns |   1.5906 ns |   1.7020 ns |      1.5906 ns |  1.00 |    0.00 |    1 | 0.0526 |      - |     440 B |        1.00 |
| LongToString          | ToString       | 10   |     81.641 ns |   0.9584 ns |   0.8965 ns |      0.9584 ns |  1.00 |    0.03 |    1 | 0.0526 |      - |     440 B |        1.00 |
| DoubleToString        | ToString       | 10   |    702.077 ns |   1.4813 ns |   1.3856 ns |      1.4813 ns |  8.56 |    0.19 |    3 | 0.0525 |      - |     440 B |        1.00 |
| DecimalToString       | ToString       | 10   |    348.666 ns |   0.6041 ns |   0.5355 ns |      0.6041 ns |  4.25 |    0.10 |    2 | 0.0525 |      - |     440 B |        1.00 |
|                       |                |      |               |             |             |                |       |         |      |        |        |           |             |
| IntMultiplication     | Multiplication | 1000 |    561.475 ns |   4.3825 ns |   3.8850 ns |      4.3825 ns |  1.00 |    0.00 |    2 |      - |      - |         - |          NA |
| LongMultiplication    | Multiplication | 1000 |    564.567 ns |   6.6763 ns |   6.2450 ns |      6.6763 ns |  1.01 |    0.02 |    2 |      - |      - |         - |          NA |
| DoubleMultiplication  | Multiplication | 1000 |    524.370 ns |   3.8248 ns |   3.5777 ns |      3.8248 ns |  0.93 |    0.01 |    1 |      - |      - |         - |          NA |
| DecimalMultiplication | Multiplication | 1000 |  2,861.327 ns |   6.0297 ns |   5.3451 ns |      6.0297 ns |  5.10 |    0.04 |    3 |      - |      - |         - |          NA |
|                       |                |      |               |             |             |                |       |         |      |        |        |           |             |
| IntParse              | Parse          | 1000 | 11,522.982 ns | 110.1938 ns | 103.0753 ns |    110.1938 ns |  1.00 |    0.00 |    1 |      - |      - |         - |          NA |
| LongParse             | Parse          | 1000 | 11,905.302 ns |  27.2268 ns |  24.1359 ns |     27.2268 ns |  1.03 |    0.01 |    2 |      - |      - |         - |          NA |
| DoubleParse           | Parse          | 1000 | 45,034.298 ns | 145.6519 ns | 129.1166 ns |    145.6519 ns |  3.91 |    0.04 |    3 |      - |      - |         - |          NA |
| DecimalParse          | Parse          | 1000 | 56,515.385 ns | 374.1036 ns | 349.9367 ns |    374.1036 ns |  4.91 |    0.06 |    4 |      - |      - |         - |          NA |
| IntTryParse           | Parse          | 1000 | 11,409.222 ns |  81.8448 ns |  76.5576 ns |     81.8448 ns |  0.99 |    0.01 |    1 |      - |      - |         - |          NA |
| LongTryParse          | Parse          | 1000 | 11,815.466 ns |  28.2369 ns |  22.0455 ns |     28.2369 ns |  1.02 |    0.01 |    2 |      - |      - |         - |          NA |
| DoubleTryParse        | Parse          | 1000 | 45,526.607 ns | 178.2550 ns | 158.0185 ns |    178.2550 ns |  3.95 |    0.03 |    3 |      - |      - |         - |          NA |
| DecimalTryParse       | Parse          | 1000 | 56,821.837 ns | 149.2260 ns | 132.2850 ns |    149.2260 ns |  4.93 |    0.04 |    4 |      - |      - |         - |          NA |
|                       |                |      |               |             |             |                |       |         |      |        |        |           |             |
| LongSum               | Sum            | 1000 |    314.429 ns |   2.2666 ns |   2.1202 ns |      2.2666 ns |  1.00 |    0.00 |    1 |      - |      - |         - |          NA |
| DoubleSum             | Sum            | 1000 |    832.908 ns |   7.0255 ns |   6.2279 ns |      7.0255 ns |  2.65 |    0.02 |    2 |      - |      - |         - |          NA |
| DecimalSum            | Sum            | 1000 |  5,021.332 ns |  15.6441 ns |  12.2139 ns |     15.6441 ns | 15.95 |    0.12 |    3 |      - |      - |         - |          NA |
|                       |                |      |               |             |             |                |       |         |      |        |        |           |             |
| IntToString           | ToString       | 1000 | 12,374.221 ns |  62.2674 ns |  58.2449 ns |     62.2674 ns |  1.00 |    0.00 |    1 | 5.2795 | 0.7477 |   44224 B |        1.00 |
| LongToString          | ToString       | 1000 | 12,469.906 ns |  58.8775 ns |  45.9676 ns |     58.8775 ns |  1.01 |    0.00 |    1 | 5.2795 | 0.7477 |   44224 B |        1.00 |
| DoubleToString        | ToString       | 1000 | 73,145.687 ns | 218.6176 ns | 193.7988 ns |    218.6176 ns |  5.91 |    0.02 |    3 | 5.2490 | 0.7324 |   44224 B |        1.00 |
| DecimalToString       | ToString       | 1000 | 38,345.588 ns | 204.5127 ns | 191.3013 ns |    204.5127 ns |  3.10 |    0.02 |    2 | 5.2490 | 0.7324 |   44224 B |        1.00 |


## Analysis
### Parsing
- `Parse` and `TryParse` has about the same performance
- Parsing of `int` and `long` about the same performance
- Parsing of `double` is ~4 times slower (comparing to `int`)
- Parsing of `decimal` is the slowest ~5 times slower (comparing to `int`)

### ToString
- Converting `int` and `long` to string has about the same performance
- Converting `double` to string is the slowest ~8 times slower (comparing to `int`)
- Converting `decimal` to string is ~4 times slower (comparing to `int`), but almost 2 times faster comparing to `double`

### Arithmetic operations
`Sum()` on `int` array causes `System.OverflowException`, so the test in not included
- Calculating sum of `long` and `double` has the same performance 
- Calculating sum of `decimal` is about 11 times slower
- Multiplication of `int`, `long` and `double` is about the same (`double` even a bit faster)
- Multiplication of `decimal` is 5-6 times slower, comparing to `int/long/double`

Arithmetic operations on `decimal` are quite expensive. Not a big surprise - precision has a cost :shrug:
