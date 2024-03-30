using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;

namespace Benchmarks;

public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        WithOption(ConfigOptions.DontOverwriteResults, true);
        WithOption(ConfigOptions.DisableLogFile, true);
        WithUnionRule(ConfigUnionRule.Union);
        WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage));
    }
}
