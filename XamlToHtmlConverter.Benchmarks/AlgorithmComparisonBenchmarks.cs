// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

#pragma warning disable CS0618
// This file intentionally uses the deprecated XmlToIrConverterLinqStyle for algorithm comparison.
// Benchmarks compare performance metrics between the legacy LINQ-based parser and the current
// recursive implementation to demonstrate performance gains from optimization efforts.

using BenchmarkDotNet.Attributes;
using XamlToHtmlConverter.Parsing;

namespace XamlToHtmlConverter.Benchmarks;

/// <summary>
/// Comparison benchmarks for testing optimization improvements.
/// Tests alternative implementations against current code to measure gains.
/// 
/// Usage:
/// - Before/After optimization: Compare LinqStyle vs Recursive parsers
/// - Algorithm comparison: Different parsing or rendering strategies
/// - Regression detection: Ensure optimizations don't become slower
/// </summary>
[SimpleJob(warmupCount: 3, targetCount: 5)]
[MemoryDiagnoser]
[RankColumn]
public class AlgorithmComparisonBenchmarks
{
    private string? v_SampleXamlPath;
    private System.Xml.Linq.XElement? v_TestElement;

    [GlobalSetup]
    public void Setup()
    {
        // Try local directory first (for Release builds)
        var localPath = Path.Combine(AppContext.BaseDirectory, "sample.xaml");
        if (File.Exists(localPath))
        {
            v_SampleXamlPath = localPath;
        }
        else
        {
            // Fall back to solution root search
            var solutionRoot = FindSolutionRoot(AppContext.BaseDirectory);
            v_SampleXamlPath = Path.Combine(solutionRoot, "XamlToHtmlConverter", "sample.xaml");
        }

        var loader = new XamlLoader();
        var doc = loader.Load(v_SampleXamlPath);
        v_TestElement = doc.Root;
    }

    /// <summary>
    /// Finds the solution root by traversing up the directory tree.
    /// </summary>
    private static string FindSolutionRoot(string startPath)
    {
        var currentPath = startPath;
        while (currentPath != null)
        {
            var projectPath = Path.Combine(currentPath, "XamlToHtmlConverter");
            if (Directory.Exists(projectPath) && File.Exists(Path.Combine(projectPath, "sample.xaml")))
                return currentPath;

            var parent = Directory.GetParent(currentPath);
            currentPath = parent?.FullName;
        }

        throw new InvalidOperationException("Could not locate solution root with XAML files.");
    }

    /// <summary>
    /// Benchmark: Recursive XML to IR conversion
    /// 
    /// Current optimized implementation.
    /// Baseline for comparison with other approaches.
    /// </summary>
    [Benchmark(Description = "XmlToIrConverterRecursive")]
    public object ConvertRecursive()
    {
        if (v_TestElement == null)
            throw new InvalidOperationException("Test element not initialized");

        IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        return converter.Convert(v_TestElement);
    }

    /// <summary>
    /// Benchmark: LINQ-style XML to IR conversion
    /// 
    /// Alternative implementation using LINQ.
    /// Compare against recursive to measure functional advantage.
    /// </summary>
    [Benchmark(Description = "XmlToIrConverterLinqStyle")]
    public object ConvertLinqStyle()
    {
        if (v_TestElement == null)
            throw new InvalidOperationException("Test element not initialized");

        IXmlToIrConverter converter = new XmlToIrConverterLinqStyle();
        return converter.Convert(v_TestElement);
    }

    /// <summary>
    /// Benchmark: Binding parsing - simple cases
    /// 
    /// Measures Span<T> optimization effectiveness
    /// for simple binding expressions (most common case).
    /// </summary>
    [Benchmark(Description = "Binding Parse: Simple")]
    public object? BindingParseSimple()
    {
        return BindingParser.Parse("{Binding Status}");
    }

    /// <summary>
    /// Benchmark: Style cache effectiveness
    /// 
    /// Measures impact of style normalization caching.
    /// High cache hit rate indicates good caching strategy.
    /// </summary>
    [Benchmark(Description = "Style Registry: Multiple Registrations")]
    public void StyleCachingEffectiveness()
    {
        var registry = new Rendering.StyleRegistry();
        
        // Register same styles multiple times
        for (int i = 0; i < 100; i++)
        {
            registry.Register("color: red; padding: 10px;");
            registry.Register("display: flex; margin: 5px;");
            registry.Register("font-size: 14px; font-weight: bold;");
        }
    }
}

/// <summary>
/// Stress test benchmarks to measure performance under high load.
/// 
/// Scenarios:
/// - High element count (deep nesting)
/// - Large style variety
/// - Complex binding expressions
/// - Heavy GC pressure
/// </summary>
[SimpleJob(warmupCount: 2, targetCount: 3)]
[MemoryDiagnoser]
public class StressBenchmarks
{
    /// <summary>
    /// Benchmark: High element count with complex styling
    /// 
    /// Simulates real-world complex UI with 500+ elements.
    /// Measures performance scaling and memory patterns.
    /// </summary>
    [Benchmark(Description = "High Element Count (500+ elements)")]
    public void HighElementCount()
    {
        var registry = new Rendering.StyleRegistry();

        // Simulate 500 elements with varied styles
        for (int i = 0; i < 500; i++)
        {
            var style = $"color: rgb({i % 255}, {(i * 2) % 255}, {(i * 3) % 255}); padding: {i % 50}px;";
            registry.Register(style);

            // Add some variation to style patterns
            if (i % 10 == 0)
                registry.Register("display: grid; gap: 10px;");
        }
    }

    /// <summary>
    /// Benchmark: High style diversity
    /// 
    /// Tests cache efficiency with many unique styles.
    /// Worst-case scenario for style deduplication.
    /// </summary>
    [Benchmark(Description = "High Style Diversity (100+ unique)")]
    public void HighStyleDiversity()
    {
        var registry = new Rendering.StyleRegistry();

        // Generate 100+ unique styles
        for (int i = 0; i < 100; i++)
        {
            var uniqueStyle = $"color: #{i:X6}; margin: {i}px; padding: {i * 2}px; border: {i % 5}px solid;";
            registry.Register(uniqueStyle);
        }
    }

    /// <summary>
    /// Benchmark: Repeated binding parsing
    /// 
    /// Measures allocation efficiency under high parsing load.
    /// Tests memory pressure from repeated binding operations.
    /// </summary>
    [Benchmark(Description = "Repeated Binding Parse (1000x)")]
    public void RepeatedBindingParsing()
    {
        for (int i = 0; i < 1000; i++)
        {
            BindingParser.Parse("{Binding Path=Item.Name, Mode=TwoWay}");
            BindingParser.Parse("{Binding RelativeSource={RelativeSource AncestorType=Window}}");
            BindingParser.Parse("{Binding Converter={StaticResource MyConverter}}");
        }
    }
}

#pragma warning restore CS0618
