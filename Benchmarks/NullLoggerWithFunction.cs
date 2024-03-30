using System.Text.Json;
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
public partial class NullLoggerWithFunction
{
    private const string Message1 = "Test message: string {String1}";
    private ILogger _logger = null!;
    private DtoModel _dtoModel = null!;
    private DtoModel2 _dtoModel2 = null!;

    [Params(1, 1_000, 100_000)] public int N;

    [GlobalSetup]
    public void Setup()
    {
        _logger = NullLoggerFactory.Instance.CreateLogger<NullLogger>();
        _dtoModel = new DtoModel {Id = 42, Message = "Hello, World!", CreatedAt = DateTime.UtcNow};
        _dtoModel2 = new DtoModel2 {Id = 42, Message = "Hello, World!", CreatedAt = DateTime.UtcNow, DtoModel = new DtoModel[42]};
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = Message1)]
    partial void LoggerMessage1(string string1);

    [Benchmark, BenchmarkCategory("SimpleModel")]
    public void Helper1()
    {
        for (var i = 0; i < N; i++)
        {
            _logger.Debug(Message1, JsonSerializer.Serialize(_dtoModel));
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("SimpleModel")]
    public void HelperWithFunction1()
    {
        for (var i = 0; i < N; i++)
        {
            _logger.Debug(Message1, () => JsonSerializer.Serialize(_dtoModel));
        }
    }

    [Benchmark, BenchmarkCategory("SimpleModel")]
    public void SourceGenerator1()
    {
        for (var i = 0; i < N; i++)
        {
            LoggerMessage1(JsonSerializer.Serialize(_dtoModel));
        }
    }

    [Benchmark, BenchmarkCategory("ComplexModel")]
    public void HelperWithComplexModel1()
    {
        for (var i = 0; i < N; i++)
        {
            _logger.Debug(Message1, JsonSerializer.Serialize(_dtoModel2));
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("ComplexModel")]
    public void HelperWithFunctionWithComplexModel1()
    {
        for (var i = 0; i < N; i++)
        {
            _logger.Debug(Message1, () => JsonSerializer.Serialize(_dtoModel2));
        }
    }

    [Benchmark, BenchmarkCategory("ComplexModel")]
    public void SourceGeneratorWithComplexModel1()
    {
        for (var i = 0; i < N; i++)
        {
            LoggerMessage1(JsonSerializer.Serialize(_dtoModel2));
        }
    }
    
    private class DtoModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private class DtoModel2
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DtoModel[] DtoModel { get; set; }
    }
}
