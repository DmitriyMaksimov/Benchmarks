# For, Any(), Exists()

SonarQube's rule `S6605`

> Both the List.Exists method and IEnumerable.Any method can be used to find the first element that satisfies a predicate in a collection. However, List.Exists can be faster than IEnumerable.Any for List objects, as well as requires significantly less memory. For small collections, the performance difference may be negligible, but for large collections, it can be noticeable. The same applies to ImmutableList and arrays too.

Let's compare performance of
- Using `for` to manually loop list's elements
- Using `Any(Predicate)` LINQ
- Using collection specific `Exists(Predicate)`

I'm using 4 tests for each of the above:
- `First` - first element satisfy the predicate (first comparison find the element)
- `Mid` - middle element satisfy the predicate (need to perform `N / 2` comparisons)
- `Last` - last element satisfy the predicate (need to perform `N` comparisons)
- `Never` - no element satisfy the predicate (need to perform `N` comparisons)


## Analysis
IMO, `Mid` is the fair test case (on average, we need to make `N / 2` comparisons).
- `Exists` is the fastest method in most tests (except "First" category)
- `For` is not as fast as expected on real scenarios ("Mid" category)
- `Any` is slowest (225% slower than `Exists`) on all tests + allocates memory


## Conclusions
- Using `for`
  - More verbose (less readable) - IMO
  - In _some_ case faster than other methods
  - Surprisingly, on average **not** the fastest method
- Using `Exists()`
  - On average fastest method 
- Using `Any()`
  - Slowest method
  - Allocates memory


Using collection specific method improves performance and allow to avoid unnecessary memory allocations.
We have a clear winner here :)


### Why `for` is slower than `Exists()`?
The fact that `for` is not the fastest methods surprise me, so I start digging to implementation of `Exists()`.

`for` using index to access elements. The indexer's implementation check boundaries
```csharp
    public T this[int index]
    {
      get
      {
        if ((uint) index >= (uint) this._size)
          ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException();
        return this._items[index];
      }
```

So, `for` take the cost of checking boundaries for each element.

On the other hand, `Exists()` have access to internal buffer (`_items`) - it does boundary check once (for the whole operation) and using the internal buffer directly. The more elements need to iterate - the bigger the saving (for example using "Never", `Exists()` faster than `for` by 22% for 10 elements, 30% for 1000).  

**PS:** please notice a nice trick (reminds me my C++ past :wink:) - instead of performing 2 checks (negative index and out of boundary check), .NET devs cast to `uint` (so a negative value became a large value, for example `(uint)-1 == uint.MaxValue`) and use a single `if` to "Kill two birds with one stone".


## Benchmark results

```
BenchmarkDotNet v0.13.12, macOS Sonoma 14.4.1 (23E224) [Darwin 23.4.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
  Job-GTELQO : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD

Server=False  

| Method      | N    | Mean         | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------ |----- |-------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| ForFirst    | 10   |     1.006 ns | 0.0100 ns | 0.0094 ns |  1.00 |    0.00 |      - |         - |          NA |
| ExistsFirst | 10   |     1.544 ns | 0.0128 ns | 0.0119 ns |  1.54 |    0.02 |      - |         - |          NA |
| AnyFirst    | 10   |     5.850 ns | 0.0269 ns | 0.0251 ns |  5.82 |    0.07 | 0.0048 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ExistsLast  | 10   |     7.826 ns | 0.0492 ns | 0.0384 ns |  0.79 |    0.01 |      - |         - |          NA |
| ForLast     | 10   |     9.954 ns | 0.1088 ns | 0.1017 ns |  1.00 |    0.00 |      - |         - |          NA |
| AnyLast     | 10   |    21.650 ns | 0.1248 ns | 0.1107 ns |  2.17 |    0.02 | 0.0048 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ExistsMid   | 10   |     5.411 ns | 0.0277 ns | 0.0259 ns |  0.61 |    0.00 |      - |         - |          NA |
| ForMid      | 10   |     8.934 ns | 0.0495 ns | 0.0413 ns |  1.00 |    0.00 |      - |         - |          NA |
| AnyMid      | 10   |    13.319 ns | 0.0963 ns | 0.0901 ns |  1.49 |    0.01 | 0.0048 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ExistsNever | 10   |     8.069 ns | 0.0725 ns | 0.0678 ns |  0.78 |    0.01 |      - |         - |          NA |
| ForNever    | 10   |    10.291 ns | 0.1166 ns | 0.1091 ns |  1.00 |    0.00 |      - |         - |          NA |
| AnyNever    | 10   |    22.517 ns | 0.1315 ns | 0.1230 ns |  2.19 |    0.03 | 0.0048 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ForFirst    | 100  |     1.008 ns | 0.0138 ns | 0.0129 ns |  1.00 |    0.00 |      - |         - |          NA |
| ExistsFirst | 100  |     1.521 ns | 0.0206 ns | 0.0193 ns |  1.51 |    0.02 |      - |         - |          NA |
| AnyFirst    | 100  |     5.824 ns | 0.0190 ns | 0.0168 ns |  5.77 |    0.07 | 0.0048 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ExistsLast  | 100  |    76.716 ns | 0.4972 ns | 0.4651 ns |  0.71 |    0.01 |      - |         - |          NA |
| ForLast     | 100  |   108.394 ns | 0.4984 ns | 0.4162 ns |  1.00 |    0.00 |      - |         - |          NA |
| AnyLast     | 100  |   212.793 ns | 0.8419 ns | 0.7875 ns |  1.96 |    0.01 | 0.0048 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ExistsMid   | 100  |    44.511 ns | 0.1951 ns | 0.1825 ns |  0.92 |    0.01 |      - |         - |          NA |
| ForMid      | 100  |    48.281 ns | 0.2197 ns | 0.1835 ns |  1.00 |    0.00 |      - |         - |          NA |
| AnyMid      | 100  |   100.082 ns | 0.6228 ns | 0.5826 ns |  2.07 |    0.02 | 0.0048 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ExistsNever | 100  |    73.239 ns | 0.0682 ns | 0.0638 ns |  0.71 |    0.00 |      - |         - |          NA |
| ForNever    | 100  |   102.668 ns | 0.0989 ns | 0.0772 ns |  1.00 |    0.00 |      - |         - |          NA |
| AnyNever    | 100  |   210.253 ns | 0.7860 ns | 0.6968 ns |  2.05 |    0.01 | 0.0048 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ForFirst    | 1000 |     1.015 ns | 0.0174 ns | 0.0154 ns |  1.00 |    0.00 |      - |         - |          NA |
| ExistsFirst | 1000 |     1.542 ns | 0.0163 ns | 0.0153 ns |  1.52 |    0.03 |      - |         - |          NA |
| AnyFirst    | 1000 |     5.867 ns | 0.0246 ns | 0.0230 ns |  5.78 |    0.09 | 0.0048 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ExistsLast  | 1000 |   695.578 ns | 1.9108 ns | 1.4918 ns |  0.68 |    0.00 |      - |         - |          NA |
| ForLast     | 1000 | 1,022.636 ns | 5.1630 ns | 4.5769 ns |  1.00 |    0.00 |      - |         - |          NA |
| AnyLast     | 1000 | 1,984.368 ns | 8.1047 ns | 6.7678 ns |  1.94 |    0.01 | 0.0038 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ExistsMid   | 1000 |   369.945 ns | 0.9837 ns | 0.8214 ns |  0.73 |    0.00 |      - |         - |          NA |
| ForMid      | 1000 |   505.924 ns | 2.9402 ns | 2.4552 ns |  1.00 |    0.00 |      - |         - |          NA |
| AnyMid      | 1000 | 1,025.559 ns | 6.8785 ns | 6.4342 ns |  2.03 |    0.02 | 0.0038 |      40 B |          NA |
|             |      |              |           |           |       |         |        |           |             |
| ExistsNever | 1000 |   685.545 ns | 0.3325 ns | 0.2947 ns |  0.70 |    0.00 |      - |         - |          NA |
| ForNever    | 1000 |   980.697 ns | 0.6751 ns | 0.5271 ns |  1.00 |    0.00 |      - |         - |          NA |
| AnyNever    | 1000 | 1,958.498 ns | 6.0861 ns | 5.3952 ns |  2.00 |    0.01 | 0.0038 |      40 B |          NA |
```
