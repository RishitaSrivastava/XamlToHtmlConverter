// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Diagnostics;
using System.Text;

namespace XamlToHtmlConverter.Benchmarks;

/// <summary>
/// Enhanced Performance Metrics Collector - integrates with existing PerformanceMetrics
/// to provide deeper insights into conversion performance.
/// 
/// Enhancements:
/// - Per-phase breakdown (loading, conversion, rendering)
/// - Memory tracking at each phase
/// - GC pressure analysis
/// - Throughput calculations
/// - Performance trend tracking
/// </summary>
public class PerformanceMetricsEnhanced
{
    #region Public Properties

    /// <summary>
    /// Unique run identifier for tracking trends.
    /// </summary>
    public string RunId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

    /// <summary>
    /// Timestamp of benchmark run.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Total elements in converted document (for scaling analysis).
    /// </summary>
    public int ElementCount { get; set; }

    /// <summary>
    /// Total CSS styles generated (indicates deduplication efficiency).
    /// </summary>
    public int StyleCount { get; set; }

    #region Phase Timings

    /// <summary>
    /// Time to load and parse XAML file (milliseconds).
    /// </summary>
    public double LoadTime { get; set; }

    /// <summary>
    /// Time to convert XML to intermediate representation (milliseconds).
    /// </summary>
    public double ConversionTime { get; set; }

    /// <summary>
    /// Time to render IR to HTML output (milliseconds).
    /// </summary>
    public double RenderTime { get; set; }

    /// <summary>
    /// Total end-to-end time (milliseconds).
    /// </summary>
    public double TotalTime { get; set; }

    #endregion

    #region Memory Metrics

    /// <summary>
    /// Memory allocated during loading phase (bytes).
    /// </summary>
    public long LoadMemory { get; set; }

    /// <summary>
    /// Memory allocated during conversion phase (bytes).
    /// </summary>
    public long ConversionMemory { get; set; }

    /// <summary>
    /// Memory allocated during rendering phase (bytes).
    /// </summary>
    public long RenderMemory { get; set; }

    /// <summary>
    /// Peak memory usage during entire conversion (bytes).
    /// </summary>
    public long PeakMemory { get; set; }

    #endregion

    #region GC Metrics

    /// <summary>
    /// Number of Generation 0 garbage collections triggered.
    /// </summary>
    public int Gen0Collections { get; set; }

    /// <summary>
    /// Number of Generation 1 garbage collections triggered.
    /// </summary>
    public int Gen1Collections { get; set; }

    /// <summary>
    /// Number of Generation 2 garbage collections triggered.
    /// </summary>
    public int Gen2Collections { get; set; }

    /// <summary>
    /// Total bytes allocated (for trend analysis).
    /// </summary>
    public long TotalAllocations { get; set; }

    #endregion

    #endregion

    #region Public Methods

    /// <summary>
    /// Captures current performance metrics snapshot.
    /// Call before and after conversion to measure actual consumption.
    /// </summary>
    public static PerformanceMetricsEnhanced CaptureSnapshot()
    {
        var gen0 = GC.CollectionCount(0);
        var gen1 = GC.CollectionCount(1);
        var gen2 = GC.CollectionCount(2);

        return new PerformanceMetricsEnhanced
        {
            Gen0Collections = gen0,
            Gen1Collections = gen1,
            Gen2Collections = gen2,
            PeakMemory = GC.GetTotalMemory(false)
        };
    }

    /// <summary>
    /// Calculates efficiency metrics (allocations per element, etc).
    /// </summary>
    public EfficiencyMetrics CalculateEfficiency()
    {
        return new EfficiencyMetrics
        {
            AllocationsPerElement = ElementCount > 0 ? TotalAllocations / (long)ElementCount : 0,
            MillisecondsPerElement = ElementCount > 0 ? TotalTime / ElementCount : 0,
            BytesPerElement = ElementCount > 0 ? PeakMemory / ElementCount : 0,
            StyleDeduplicationRatio = StyleCount > 0 ? (ElementCount * 100.0) / StyleCount : 0,
            TimeBreakdown = new PhaseBreakdown
            {
                LoadPercent = TotalTime > 0 ? (LoadTime / TotalTime) * 100 : 0,
                ConversionPercent = TotalTime > 0 ? (ConversionTime / TotalTime) * 100 : 0,
                RenderPercent = TotalTime > 0 ? (RenderTime / TotalTime) * 100 : 0
            }
        };
    }

    /// <summary>
    /// Generates human-readable performance report.
    /// </summary>
    public string GenerateReport()
    {
        var sb = new StringBuilder();
        var efficiency = CalculateEfficiency();

        sb.AppendLine("╔════════════════════════════════════════════╗");
        sb.AppendLine("║   Enhanced Performance Metrics Report      ║");
        sb.AppendLine("╚════════════════════════════════════════════╝");
        sb.AppendLine();

        sb.AppendLine($"Run ID:        {RunId}");
        sb.AppendLine($"Timestamp:     {Timestamp:yyyy-MM-dd HH:mm:ss UTC}");
        sb.AppendLine();

        sb.AppendLine("╔─ EXECUTION TIME ─────────────────────────╗");
        sb.AppendLine($"  Load Time      : {LoadTime:F2} ms ({efficiency.TimeBreakdown.LoadPercent:F1}%)");
        sb.AppendLine($"  Conversion Time: {ConversionTime:F2} ms ({efficiency.TimeBreakdown.ConversionPercent:F1}%)");
        sb.AppendLine($"  Render Time    : {RenderTime:F2} ms ({efficiency.TimeBreakdown.RenderPercent:F1}%)");
        sb.AppendLine($"  ─────────────────────────────────────");
        sb.AppendLine($"  Total Time     : {TotalTime:F2} ms");
        sb.AppendLine();

        sb.AppendLine("╔─ MEMORY USAGE ───────────────────────────╗");
        sb.AppendLine($"  Load Memory       : {FormatBytes(LoadMemory)}");
        sb.AppendLine($"  Conversion Memory : {FormatBytes(ConversionMemory)}");
        sb.AppendLine($"  Render Memory     : {FormatBytes(RenderMemory)}");
        sb.AppendLine($"  ─────────────────────────────────────");
        sb.AppendLine($"  Peak Memory       : {FormatBytes(PeakMemory)}");
        sb.AppendLine($"  Total Allocations : {TotalAllocations:N0} bytes");
        sb.AppendLine();

        sb.AppendLine("╔─ GARBAGE COLLECTION ─────────────────────╗");
        sb.AppendLine($"  Gen0 Collections: {Gen0Collections}");
        sb.AppendLine($"  Gen1 Collections: {Gen1Collections}");
        sb.AppendLine($"  Gen2 Collections: {Gen2Collections}");
        sb.AppendLine();

        sb.AppendLine("╔─ DOCUMENT METRICS ───────────────────────╗");
        sb.AppendLine($"  Element Count : {ElementCount}");
        sb.AppendLine($"  Style Count   : {StyleCount}");
        sb.AppendLine();

        sb.AppendLine("╔─ EFFICIENCY ANALYSIS ────────────────────╗");
        sb.AppendLine($"  Allocs/Element     : {efficiency.AllocationsPerElement:N0}");
        sb.AppendLine($"  ms/Element         : {efficiency.MillisecondsPerElement:F4}");
        sb.AppendLine($"  Bytes/Element      : {efficiency.BytesPerElement:N0}");
        sb.AppendLine($"  Style Ratio        : {efficiency.StyleDeduplicationRatio:F2}:1");
        sb.AppendLine("  (Higher ratio = better deduplication)");
        sb.AppendLine();

        return sb.ToString();
    }

    /// <summary>
    /// Compares two metric snapshots to calculate delta.
    /// </summary>
    public static MetricsDelta Compare(PerformanceMetricsEnhanced before, PerformanceMetricsEnhanced after)
    {
        return new MetricsDelta
        {
            TimeDelta = after.TotalTime - before.TotalTime,
            TimePercentChange = before.TotalTime > 0 ? ((after.TotalTime - before.TotalTime) / before.TotalTime) * 100 : 0,
            MemoryDelta = after.PeakMemory - before.PeakMemory,
            MemoryPercentChange = before.PeakMemory > 0 ? ((after.PeakMemory - before.PeakMemory) / before.PeakMemory) * 100 : 0,
            AllocationsDelta = after.TotalAllocations - before.TotalAllocations,
            GCPressureDelta = (after.Gen0Collections + after.Gen1Collections + after.Gen2Collections) 
                - (before.Gen0Collections + before.Gen1Collections + before.Gen2Collections)
        };
    }

    #endregion

    #region Private Methods

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        
        return $"{len:F2} {sizes[order]}";
    }

    #endregion
}

/// <summary>
/// Calculated efficiency metrics for performance analysis.
/// </summary>
public class EfficiencyMetrics
{
    public long AllocationsPerElement { get; set; }
    public double MillisecondsPerElement { get; set; }
    public long BytesPerElement { get; set; }
    public double StyleDeduplicationRatio { get; set; }
    public PhaseBreakdown TimeBreakdown { get; set; } = new();
}

/// <summary>
/// Time breakdown across conversion phases.
/// </summary>
public class PhaseBreakdown
{
    public double LoadPercent { get; set; }
    public double ConversionPercent { get; set; }
    public double RenderPercent { get; set; }
}

/// <summary>
/// Delta metrics between two performance runs.
/// </summary>
public class MetricsDelta
{
    public double TimeDelta { get; set; }
    public double TimePercentChange { get; set; }
    public long MemoryDelta { get; set; }
    public double MemoryPercentChange { get; set; }
    public long AllocationsDelta { get; set; }
    public int GCPressureDelta { get; set; }
}
