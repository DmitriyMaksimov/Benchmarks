using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace Benchmarks;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams, BenchmarkLogicalGroupRule.ByCategory)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[GcServer]
public class ForVsAnyVsExists
{
    [Params(10, 100, 1_000)] public static int N { get; set; }

    private List<int> _data = null!;

    private readonly Func<int, bool> _funcFirst = n => n == 1;
    private readonly Func<int, bool> _funcMid = n => n == N / 2;
    private readonly Func<int, bool> _funcLast = n => n == N;
    private readonly Func<int, bool> _funcNever = n => n == N + 1;
    private readonly Predicate<int> _predicateFirst = n => n == 1;
    private readonly Predicate<int> _predicateMid = n => n == N / 2;
    private readonly Predicate<int> _predicateLast = n => n == N;
    private readonly Predicate<int> _predicateNever = n => n == N + 1;

    [GlobalSetup]
    public void Setup()
    {
        _data = Enumerable.Range(1, N).ToList();
    }

    [Benchmark, BenchmarkCategory("First")]
    public void AnyFirst()
    {
        _ = _data.Any(_funcFirst);
    }

    [Benchmark, BenchmarkCategory("Mid")]
    public void AnyMid()
    {
        _ = _data.Any(_funcMid);
    }

    [Benchmark, BenchmarkCategory("Last")]
    public void AnyLast()
    {
        _ = _data.Any(_funcLast);
    }

    [Benchmark, BenchmarkCategory("Never")]
    public void AnyNever()
    {
        _ = _data.Any(_funcNever);
    }

    [Benchmark, BenchmarkCategory("First")]
    public void ExistsFirst()
    {
        _ = _data.Exists(_predicateFirst);
    }

    [Benchmark, BenchmarkCategory("Mid")]
    public void ExistsMid()
    {
        _ = _data.Exists(_predicateMid);
    }

    [Benchmark, BenchmarkCategory("Last")]
    public void ExistsLast()
    {
        _ = _data.Exists(_predicateLast);
    }

    [Benchmark, BenchmarkCategory("Never")]
    public void ExistsNever()
    {
        _ = _data.Exists(_predicateNever);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("First")]
    public void ForFirst()
    {
        for (var i = 0; i < N; i++)
        {
            if (_predicateFirst(_data[i])) return;
        }
    }
    
    [Benchmark(Baseline = true), BenchmarkCategory("Mid")]
    public void ForMid()
    {
        for (var i = 0; i < N; i++)
        {
            if (_predicateMid(_data[i])) return;
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Last")]
    public void ForLast()
    {
        for (var i = 0; i < N; i++)
        {
            if (_predicateLast(_data[i])) return;
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Never")]
    public void ForNever()
    {
        for (var i = 0; i < N; i++)
        {
            if (_predicateNever(_data[i])) return;
        }
    }
}