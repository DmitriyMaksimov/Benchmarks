using BenchmarkDotNet.Running;

// BenchmarkRunner.Run<Benchmarks.StaticMethods>();
// BenchmarkRunner.Run<Benchmarks.Logging.Logging>();
// BenchmarkRunner.Run<Benchmarks.IntLongDoubleDecimal>();
// BenchmarkRunner.Run<Benchmarks.NullLogger>();
// BenchmarkRunner.Run<Benchmarks.NullLoggerInsideLoop>();
BenchmarkRunner.Run<Benchmarks.NullLoggerWithFunction>();
