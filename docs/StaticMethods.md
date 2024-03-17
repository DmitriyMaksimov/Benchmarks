# StaticMethods

Compare call of `static`, `virtual` and non static methods

## Results

```
BenchmarkDotNet v0.13.12, macOS Sonoma 14.4 (23E214) [Darwin 23.4.0]
Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
.NET SDK 8.0.201
[Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
DefaultJob : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
```

| Method    | Mean      | Error     | StdDev    | Median    |
|---------- |----------:|----------:|----------:|----------:|
| NonStatic | 0.0509 ns | 0.0040 ns | 0.0038 ns | 0.0514 ns |
| Virtual   | 0.0085 ns | 0.0019 ns | 0.0018 ns | 0.0087 ns |
| Static    | 0.0001 ns | 0.0004 ns | 0.0004 ns | 0.0000 ns |

```
// * Warnings *
MultimodalDistribution
StaticMethods.Virtual: Default -> It seems that the distribution is bimodal (mValue = 3.25)
ZeroMeasurement
StaticMethods.Virtual: Default -> The method duration is indistinguishable from the empty method duration
StaticMethods.Static: Default  -> The method duration is indistinguishable from the empty method duration
```

## Analysis
It's hard to compare results on sub-nanosecond numbers (1 Nanosecond == 0.000000001 sec) :smile:

Surprising that `virtual` is faster then `NonStatic` :shrug:

Saying that, when we check IL code of these methods, we see that `static` has trivial implementation - no passing `this` and no virtual calls. Still puzled why `NonStatic` produces a virtual call :thinking: 

### IL
#### NonStatic()
    IL_0000: ldarg.0      // this
    IL_0001: ldfld        class Benchmarks.StaticMethods/Implementation Benchmarks.StaticMethods::_implementation
    IL_0006: callvirt     instance int32 Benchmarks.StaticMethods/Implementation::NonStatic()
    IL_000b: ret

#### Virtual()
    IL_0000: ldarg.0      // this
    IL_0001: ldfld        class Benchmarks.StaticMethods/Implementation Benchmarks.StaticMethods::_implementation
    IL_0006: callvirt     instance int32 Benchmarks.StaticMethods/Implementation::Virtual()
    IL_000b: ret

#### Static()
    IL_0000: call         int32 Benchmarks.StaticMethods/Implementation::Static()
    IL_0005: ret


## Conclusions
Despite the fact, that static call is 500 times faster, I doubt that using `static` will give any significant performance improvements.

Saying that, using static it a trivial change - so it's a free improvement. Also, keep in mind that using `static` allows Roslyn/Jit to perform other code optimizations (for example, inlining) which _could_ improve performance dramatically.

