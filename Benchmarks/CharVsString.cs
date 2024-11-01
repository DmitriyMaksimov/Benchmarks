using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
[GcServer]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
[Config(typeof(BenchmarkConfig))]
public class CharVsString
{
    private string _str = null!;
    
    [Params(10, 100, 1_000)] public int N;

    [GlobalSetup]
    public void Setup()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        _str = new string(Enumerable.Repeat(chars, N).Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }

    [Benchmark]
    public bool ContainsChar() => _str.Contains('_');

    [Benchmark]
    public bool ContainsCharOrdinal() => _str.Contains('_', StringComparison.Ordinal);

    [Benchmark]
    public bool ContainsCharInvariant() => _str.Contains('_', StringComparison.InvariantCulture);

    [Benchmark]
    public bool ContainsStr() => _str.Contains("_");

    [Benchmark]
    public bool ContainsStrOrdinal() => _str.Contains("_", StringComparison.Ordinal);

    [Benchmark]
    public bool ContainsStrInvariant() => _str.Contains("_", StringComparison.InvariantCulture);
    
    [Benchmark]
    public bool StartsWithChar() => _str.StartsWith('_');
    
    [Benchmark]
    public bool StartsWithStr() => _str.StartsWith("_");
    
    [Benchmark]
    public bool StartsWithStrOrdinal() => _str.StartsWith("_", StringComparison.Ordinal);
    
    [Benchmark]
    public bool StartsWithStrInvariant() => _str.StartsWith("_", StringComparison.InvariantCulture);
    
    [Benchmark]
    public bool EndsWithChar() => _str.EndsWith('_');
    
    [Benchmark]
    public bool EndsWithStr() => _str.EndsWith("_");
    
    [Benchmark]
    public bool EndsWithStrOrdinal() => _str.EndsWith("_", StringComparison.Ordinal);
    
    [Benchmark]
    public bool EndsWithStrInvariant() => _str.EndsWith("_", StringComparison.InvariantCulture);
    
    [Benchmark]
    public int IndexOfChar() => _str.IndexOf('_');
    
    [Benchmark]
    public int IndexOfStr() => _str.IndexOf("_");
    
    [Benchmark]
    public int IndexOfStrOrdinal() => _str.IndexOf("_", StringComparison.Ordinal);
    
    [Benchmark]
    public int IndexOfStrInvariant() => _str.IndexOf("_", StringComparison.InvariantCulture);
    
    [Benchmark]
    public int LastIndexOfChar() => _str.LastIndexOf('_');
    
    [Benchmark]
    public int LastIndexOfStr() => _str.LastIndexOf("_");
    
    [Benchmark]
    public int LastIndexOfStrOrdinal() => _str.LastIndexOf("_", StringComparison.Ordinal);
    
    [Benchmark]
    public int LastIndexOfStrInvariant() => _str.LastIndexOf("_", StringComparison.InvariantCulture);
}
