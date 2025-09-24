using System.Security.Cryptography;
using System.IO.Hashing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace Benchmarks;

[MemoryDiagnoser]
[GcServer]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
[HideColumns(Column.Error, Column.StdDev, Column.RatioSD)]
[Config(typeof(BenchmarkConfig))]
public class Hashing
{
    private byte[] _data = null!;

    [Params(16, 64, 256, 1024, 4096, 16384)]
    public int DataLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var rnd = RandomNumberGenerator.Create();
        _data = new byte[DataLength];
        rnd.GetBytes(_data);
    }

    [Benchmark]
    public byte[] SHA256_Hash()
    {
        return SHA256.HashData(_data);
    }

    [Benchmark]
    public byte[] SHA3_256_Hash()
    {
        return SHA3_256.HashData(_data);
    }

    [Benchmark]
    public byte[] SHA384_Hash()
    {
        return SHA384.HashData(_data);
    }

    [Benchmark]
    public byte[] SHA3_384_Hash()
    {
        return SHA3_384.HashData(_data);
    }

    [Benchmark]
    public byte[] SHA512_Hash()
    {
        return SHA512.HashData(_data);
    }

    [Benchmark]
    public byte[] SHA3_512_Hash()
    {
        return SHA3_512.HashData(_data);
    }

    [Benchmark(Baseline = true)]
    public byte[] XXHash3_Hash()
    {
        return XxHash3.Hash(_data);
    }

    [Benchmark]
    public byte[] XXHash32_Hash()
    {
        return XxHash32.Hash(_data);
    }

    [Benchmark]
    public byte[] XXHash64_Hash()
    {
        return XxHash64.Hash(_data);
    }

    [Benchmark]
    public byte[] XXHash128_Hash()
    {
        return XxHash128.Hash(_data);
    }
}