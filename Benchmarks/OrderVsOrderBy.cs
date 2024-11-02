using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Benchmarks;

[MemoryDiagnoser]
[GcServer]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
[SimpleJob(RuntimeMoniker.Net80)]
[HideColumns(Column.Error, Column.StdDev, Column.RatioSD)]
[Config(typeof(BenchmarkConfig))]
public class OrderVsOrderBy
{
    private List<int> _list = null!;

    public enum InitOrder
    {
        Ascending,
        Descending,
        Random
    }

    [Params(10, 100, 1000, 10_000)] public int N;
    [ParamsAllValues] public InitOrder InitializationOrder;

    [GlobalSetup]
    public void Setup()
    {
        var start = Random.Shared.Next();

        var enumerable = InitializationOrder switch
        {
            InitOrder.Ascending => Enumerable.Range(start, N),
            InitOrder.Descending => Enumerable.Range(start, N).Reverse(),
            InitOrder.Random => Enumerable.Range(0, N).Select(_ => Random.Shared.Next()),
            _ => throw new ArgumentOutOfRangeException()
        };

        _list = enumerable.ToList();
    }

    [Benchmark]
    public int OrderBy() => _list.OrderBy(x => x).ToList().Count;

    [Benchmark(Baseline = true)]
    public int Order() => _list.Order().ToList().Count;
}
