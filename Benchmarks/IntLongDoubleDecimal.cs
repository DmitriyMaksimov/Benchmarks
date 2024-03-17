using System.Globalization;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
[RankColumn, CategoriesColumn, ConfidenceIntervalErrorColumn]
[GcServer]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams, BenchmarkLogicalGroupRule.ByCategory)]
public class IntLongDoubleDecimal
{
    private string[] _arrayString = null!;
    private int[] _arrayInt = null!;
    private long[] _arrayLong = null!;
    private double[] _arrayDouble = null!;
    private decimal[] _arrayDecimal = null!;
    [Params(10, 1000)] public int N;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _arrayString = new string[N];
        _arrayInt = new int[N];
        _arrayLong = new long[N];
        _arrayDouble = new double[N];
        _arrayDecimal = new decimal[N];

        for (var i = 0; i < N; i++)
        {
            var rnd = random.Next();
            _arrayString[i] = rnd.ToString();
            _arrayInt[i] = rnd;
            _arrayLong[i] = rnd;
            _arrayDouble[i] = rnd;
            _arrayDecimal[i] = rnd;
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Parse")]
    public void IntParse()
    {
        for (var i = 0; i < _arrayString.Length; i++)
        {
            _arrayInt[i] = int.Parse(_arrayString[i]);
        }
    }

    [Benchmark, BenchmarkCategory("Parse")]
    public void LongParse()
    {
        for (var i = 0; i < _arrayString.Length; i++)
        {
            _arrayLong[i] = long.Parse(_arrayString[i]);
        }
    }

    [Benchmark, BenchmarkCategory("Parse")]
    public void DoubleParse()
    {
        for (var i = 0; i < _arrayString.Length; i++)
        {
            _arrayDouble[i] = double.Parse(_arrayString[i]);
        }
    }

    [Benchmark, BenchmarkCategory("Parse")]
    public void DecimalParse()
    {
        for (var i = 0; i < _arrayString.Length; i++)
        {
            _arrayDecimal[i] = decimal.Parse(_arrayString[i]);
        }
    }

    [Benchmark, BenchmarkCategory("Parse")]
    public void IntTryParse()
    {
        for (var i = 0; i < _arrayString.Length; i++)
        {
            int.TryParse(_arrayString[i], out _arrayInt[i]);
        }
    }

    [Benchmark, BenchmarkCategory("Parse")]
    public void LongTryParse()
    {
        for (var i = 0; i < _arrayString.Length; i++)
        {
            long.TryParse(_arrayString[i], out _arrayLong[i]);
        }
    }

    [Benchmark, BenchmarkCategory("Parse")]
    public void DoubleTryParse()
    {
        for (var i = 0; i < _arrayString.Length; i++)
        {
            double.TryParse(_arrayString[i], out _arrayDouble[i]);
        }
    }

    [Benchmark, BenchmarkCategory("Parse")]
    public void DecimalTryParse()
    {
        for (var i = 0; i < _arrayString.Length; i++)
        {
            decimal.TryParse(_arrayString[i], out _arrayDecimal[i]);
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("ToString")]
    public void IntToString()
    {
        for (var i = 0; i < _arrayInt.Length; i++)
        {
            _arrayString[i] = _arrayInt[i].ToString();
        }
    }

    [Benchmark, BenchmarkCategory("ToString")]
    public void LongToString()
    {
        for (var i = 0; i < _arrayInt.Length; i++)
        {
            _arrayString[i] = _arrayInt[i].ToString();
        }
    }

    [Benchmark, BenchmarkCategory("ToString")]
    public void DoubleToString()
    {
        for (var i = 0; i < _arrayDouble.Length; i++)
        {
            _arrayString[i] = _arrayDouble[i].ToString(CultureInfo.InvariantCulture);
        }
    }

    [Benchmark, BenchmarkCategory("ToString")]
    public void DecimalToString()
    {
        for (var i = 0; i < _arrayDecimal.Length; i++)
        {
            _arrayString[i] = _arrayDecimal[i].ToString(CultureInfo.InvariantCulture);
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Sum")]
    public long LongSum() => _arrayLong.Sum();

    [Benchmark, BenchmarkCategory("Sum")]
    public double DoubleSum() => _arrayDouble.Sum();

    [Benchmark, BenchmarkCategory("Sum")]
    public decimal DecimalSum() => _arrayDecimal.Sum();

    [Benchmark(Baseline = true), BenchmarkCategory("Multiplication")]
    public void IntMultiplication()
    {
        for (var i = 0; i < _arrayInt.Length; i++)
        {
            _arrayInt[i] *= 42;
        }
    }

    [Benchmark, BenchmarkCategory("Multiplication")]
    public void LongMultiplication()
    {
        for (var i = 0; i < _arrayInt.Length; i++)
        {
            _arrayInt[i] *= 42;
        }
    }

    [Benchmark, BenchmarkCategory("Multiplication")]
    public void DoubleMultiplication()
    {
        for (var i = 0; i < _arrayDouble.Length; i++)
        {
            _arrayDouble[i] *= 42;
        }
    }

    [Benchmark, BenchmarkCategory("Multiplication")]
    public void DecimalMultiplication()
    {
        for (var i = 0; i < _arrayDecimal.Length; i++)
        {
            _arrayDecimal[i] = _arrayInt[i] * 2M;
        }
    }
}
