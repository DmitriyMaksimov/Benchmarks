using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Attributes;
using Benchmarks.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Benchmarks;

[MemoryDiagnoser]
[RankColumn, CategoriesColumn, ConfidenceIntervalErrorColumn]
[GcServer]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams, BenchmarkLogicalGroupRule.ByCategory)]
[Config(typeof(BenchmarkConfig))]
public class NullLoggerInsideLoop
{
    private const string Message3 = "Test message: int {Int1}, {String1} and {Time}";

    private ILogger _logger = NullLoggerFactory.Instance.CreateLogger<NullLoggerInsideLoop>();
    private readonly DateTime _startTime = DateTime.UtcNow;
    private const int IntValue = 42;
    private const string StringValue = "Hello, World!";

    [Params(100, 1_000, 1_000_000, 1_000_000_000)] public int N;

    [Benchmark]
    public void Helper3()
    {
        for (var i = 0; i < N; i++)
        {
            _logger.Debug(Message3, IntValue, StringValue, _startTime);
        }
    }
}
