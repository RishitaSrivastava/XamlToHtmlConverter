// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System;
using System.Diagnostics;

namespace XamlToHtmlConverter;

/// <summary>
/// Collects and records performance metrics for the XAML to HTML conversion pipeline.
/// Tracks execution time for each stage (loading, IR conversion, rendering)
/// and total document construction. Provides observability for production deployments.
/// </summary>
public class PerformanceMetrics
{
    #region Public Properties

    /// <summary>
    /// Total time for XAML loading from disk (includes XML parsing).
    /// </summary>
    public TimeSpan LoadingTime { get; set; }

    /// <summary>
    /// Total time for XML to IR conversion (includes all IR construction).
    /// </summary>
    public TimeSpan ConversionTime { get; set; }

    /// <summary>
    /// Total time for IR to HTML rendering (includes CSS generation, attribute emission).
    /// </summary>
    public TimeSpan RenderingTime { get; set; }

    /// <summary>
    /// Total elapsed time for the complete conversion pipeline.
    /// </summary>
    public TimeSpan TotalTime { get; set; }

    /// <summary>
    /// Number of elements in the IR tree (for scaling analysis).
    /// </summary>
    public int ElementCount { get; set; }

    /// <summary>
    /// Number of CSS styles generated and deduplicated.
    /// </summary>
    public int StyleCount { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a formatted string representation of all recorded metrics.
    /// Useful for console output and log files.
    /// </summary>
    /// <returns>A multi-line string with all metrics and timing information.</returns>
    public override string ToString()
    {
        return $@"
╔════════════════════════════════════════════╗
║     XAML to HTML Conversion Metrics        ║
╚════════════════════════════════════════════╝

  Loading Time      : {LoadingTime.TotalMilliseconds:F2} ms
  Conversion Time   : {ConversionTime.TotalMilliseconds:F2} ms
  Rendering Time    : {RenderingTime.TotalMilliseconds:F2} ms
  ─────────────────────────────────────────
  Total Time        : {TotalTime.TotalMilliseconds:F2} ms

  Element Count     : {ElementCount:D}
  Style Count       : {StyleCount:D}

╔════════════════════════════════════════════╗
║            Stage Breakdown                 ║
╚════════════════════════════════════════════╝

  Loading   : {GetPercentage(LoadingTime, TotalTime):F1}%
  Conversion: {GetPercentage(ConversionTime, TotalTime):F1}%
  Rendering : {GetPercentage(RenderingTime, TotalTime):F1}%
";
    }

    #endregion

    #region Private Methods

    private static double GetPercentage(TimeSpan part, TimeSpan total)
    {
        if (total.TotalMilliseconds == 0)
            return 0;
        return (part.TotalMilliseconds / total.TotalMilliseconds) * 100;
    }

    #endregion
}
