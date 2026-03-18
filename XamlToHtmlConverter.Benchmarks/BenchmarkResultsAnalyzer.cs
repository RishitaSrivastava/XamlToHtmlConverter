// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using System.Text.Json;

namespace XamlToHtmlConverter.Benchmarks;

/// <summary>
/// Utility for benchmarking result analysis and comparison.
/// 
/// Features:
/// - Parse BenchmarkDotNet JSON output
/// - Compare versions (baseline vs current)
/// - Detect performance regressions
/// - Generate regression reports
/// - Track performance trends over time
/// </summary>
public class BenchmarkResultsAnalyzer
{
    private readonly string v_ResultsDirectory;
    private const string ResultsFileName = "BenchmarkResults.json";

    public BenchmarkResultsAnalyzer(string resultsDirectory = "./BenchmarkResults")
    {
        v_ResultsDirectory = resultsDirectory;
    }

    /// <summary>
    /// Compares current benchmark results with baseline (previous run).
    /// Detects performance regressions and improvements.
    /// </summary>
    public BenchmarkComparison Compare()
    {
        var currentResults = LoadLatestResults();
        var baselineResults = LoadBaselineResults();

        if (baselineResults == null)
            return new BenchmarkComparison { IsFirstRun = true, Benchmarks = new() };

        return AnalyzeComparison(baselineResults, currentResults ?? new());
    }

    /// <summary>
    /// Loads benchmark results from the latest run.
    /// </summary>
    private Dictionary<string, BenchmarkMetric>? LoadLatestResults()
    {
        var latestFile = FindLatestResultFile();
        if (latestFile == null)
            return null;

        return ParseResultsFile(latestFile);
    }

    /// <summary>
    /// Loads baseline benchmark results from backup.
    /// </summary>
    private Dictionary<string, BenchmarkMetric>? LoadBaselineResults()
    {
        var baselineFile = Path.Combine(v_ResultsDirectory, "baseline.json");
        if (!File.Exists(baselineFile))
            return null;

        return ParseResultsFile(baselineFile);
    }

    /// <summary>
    /// Finds the latest benchmark results file.
    /// </summary>
    private string? FindLatestResultFile()
    {
        if (!Directory.Exists(v_ResultsDirectory))
            return null;

        return Directory.GetFiles(v_ResultsDirectory, "*.json")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .FirstOrDefault();
    }

    /// <summary>
    /// Parses benchmark results JSON file.
    /// </summary>
    private Dictionary<string, BenchmarkMetric> ParseResultsFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        var results = JsonSerializer.Deserialize<BenchmarkResultsData>(content);
        
        var dict = new Dictionary<string, BenchmarkMetric>();
        if (results?.Benchmarks != null)
        {
            foreach (var b in results.Benchmarks)
            {
                if (!string.IsNullOrEmpty(b.Method))
                {
                    dict[b.Method] = new BenchmarkMetric
                    {
                        Mean = b.Mean,
                        StdDev = b.StdDev,
                        Median = b.Median,
                        Min = b.Min,
                        Max = b.Max,
                        AllocationsPerOperation = b.AllocatedBytes ?? 0
                    };
                }
            }
        }
        return dict;
    }

    /// <summary>
    /// Analyzes comparison between baseline and current results.
    /// </summary>
    private BenchmarkComparison AnalyzeComparison(
        Dictionary<string, BenchmarkMetric> baseline,
        Dictionary<string, BenchmarkMetric> current)
    {
        var comparison = new BenchmarkComparison { Benchmarks = new() };

        foreach (var bench in current)
        {
            if (baseline.TryGetValue(bench.Key, out var baselineMetric))
            {
                var improvement = CalculateImprovement(baselineMetric, bench.Value);
                comparison.Benchmarks[bench.Key] = improvement;

                if (improvement.RegressionDetected)
                    comparison.RegressionCount++;
                else if (improvement.TimeChangePercent < -5)
                    comparison.ImprovementCount++;
            }
        }

        return comparison;
    }

    /// <summary>
    /// Calculates improvement metrics between two benchmark runs.
    /// </summary>
    private BenchmarkImprovement CalculateImprovement(
        BenchmarkMetric baseline,
        BenchmarkMetric current)
    {
        var timeChange = ((current.Mean - baseline.Mean) / baseline.Mean) * 100;
        var allocChange = ((current.AllocationsPerOperation - baseline.AllocationsPerOperation) 
            / (baseline.AllocationsPerOperation > 0 ? baseline.AllocationsPerOperation : 1)) * 100;

        return new BenchmarkImprovement
        {
            TimeChangePercent = timeChange,
            MemoryChangePercent = allocChange,
            RegressionDetected = timeChange > 5, // 5% regression threshold
            SignificantImprovement = timeChange < -10 // -10% improvement threshold
        };
    }

    /// <summary>
    /// Generates a detailed comparison report.
    /// </summary>
    public string GenerateReport(BenchmarkComparison comparison)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("╔════════════════════════════════════════════╗");
        sb.AppendLine("║    Benchmark Comparison Report             ║");
        sb.AppendLine("╚════════════════════════════════════════════╝");
        sb.AppendLine($"\nGenerated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        if (comparison.IsFirstRun)
        {
            sb.AppendLine("\n✅ First benchmark run - no baseline for comparison");
            sb.AppendLine("Run again to see improvement tracking.");
        }
        else
        {
            sb.AppendLine($"\n📊 Comparison Summary:");
            sb.AppendLine($"  Improvements: {comparison.ImprovementCount}");
            sb.AppendLine($"  Regressions:  {comparison.RegressionCount} ⚠️");
            sb.AppendLine();

            foreach (var benchmark in comparison.Benchmarks)
            {
                var timeStr = benchmark.Value.TimeChangePercent < 0 ? "✅" : benchmark.Value.TimeChangePercent > 5 ? "❌" : "⚠️";
                sb.AppendLine($"  • {benchmark.Key}");
                sb.AppendLine($"    Time: {benchmark.Value.TimeChangePercent:+0.00;-0.00}% {timeStr}");
                sb.AppendLine($"    Memory: {benchmark.Value.MemoryChangePercent:+0.00;-0.00}%");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Saves current results as baseline for future comparisons.
    /// </summary>
    public void SaveAsBaseline(string sourceFile)
    {
        Directory.CreateDirectory(v_ResultsDirectory);
        var baselineFile = Path.Combine(v_ResultsDirectory, "baseline.json");
        File.Copy(sourceFile, baselineFile, overwrite: true);
    }
}

/// <summary>
/// Benchmark comparison metadata.
/// </summary>
public class BenchmarkComparison
{
    public bool IsFirstRun { get; set; }
    public Dictionary<string, BenchmarkImprovement> Benchmarks { get; set; } = new();
    public int ImprovementCount { get; set; }
    public int RegressionCount { get; set; }
}

/// <summary>
/// Individual benchmark improvement metrics.
/// </summary>
public class BenchmarkImprovement
{
    public double TimeChangePercent { get; set; }
    public double MemoryChangePercent { get; set; }
    public bool RegressionDetected { get; set; }
    public bool SignificantImprovement { get; set; }
}

/// <summary>
/// Individual benchmark metric snapshot.
/// </summary>
public class BenchmarkMetric
{
    public double Mean { get; set; }
    public double StdDev { get; set; }
    public double Median { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double AllocationsPerOperation { get; set; }
}

/// <summary>
/// BenchmarkDotNet JSON result structure.
/// </summary>
public class BenchmarkResultsData
{
    public BenchmarkResult[]? Benchmarks { get; set; }
}

/// <summary>
/// Individual benchmark result from BenchmarkDotNet.
/// </summary>
public class BenchmarkResult
{
    public string? Method { get; set; }
    public double Mean { get; set; }
    public double StdDev { get; set; }
    public double Median { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double? AllocatedBytes { get; set; }
}
