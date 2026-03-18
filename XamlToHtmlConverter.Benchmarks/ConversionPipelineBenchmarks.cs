// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Diagnostics;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Benchmarks;

/// <summary>
/// Performance benchmarks for the complete XAML to HTML conversion pipeline.
/// Measures: execution time, memory allocations, GC pressure, and throughput.
/// 
/// Benchmark Scope:
/// - Full pipeline: XAML loading → IR conversion → HTML rendering
/// - Includes style deduplication and resource resolution
/// 
/// Configuration:
/// - Warmup: 3 iterations
/// - Target: 5 iterations
/// - Memory diagnostics: Enabled (tracks GC allocations)
/// </summary>
[SimpleJob(warmupCount: 3, targetCount: 5)]
[MemoryDiagnoser]
[RankColumn]
public class ConversionPipelineBenchmarks
{
    private string? v_SampleXamlPath;
    private string? v_LargeXamlPath;

    [GlobalSetup]
    public void Setup()
    {
        // Use sample XAML files from the project
        var baseDir = AppContext.BaseDirectory;
        v_SampleXamlPath = Path.Combine(baseDir, "..", "..", "..", "XamlToHtmlConverter", "sample.xaml");
        v_LargeXamlPath = Path.Combine(baseDir, "..", "..", "..", "XamlToHtmlConverter", "sample2.xaml");
    }

    /// <summary>
    /// Benchmark: Complete conversion of sample XAML (small document)
    /// 
    /// Measures:
    /// - Time to convert typical XAML (100-200 elements)
    /// - Memory allocated during conversion
    /// - GC collections triggered
    /// - Allocations per element
    /// </summary>
    [Benchmark(Description = "Sample XAML Conversion")]
    public string ConvertSampleXaml()
    {
        var loader = new XamlLoader();
        var document = loader.Load(v_SampleXamlPath);

        if (document.Root == null)
            throw new InvalidOperationException("No root element");

        // Phase 1: XML to IR Conversion
        IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        var ir = converter.Convert(document.Root);

        // Phase 2: HTML Rendering
        var renderer = HtmlRendererFactory.Create();
        var html = renderer.RenderDocument(ir);

        return html;
    }

    /// <summary>
    /// Benchmark: Complete conversion of large XAML (complex document)
    /// 
    /// Measures:
    /// - Performance degradation with document complexity
    /// - Memory scaling characteristics
    /// - GC pressure on larger inputs
    /// - Practical throughput for real-world usage
    /// </summary>
    [Benchmark(Description = "Large XAML Conversion")]
    public string ConvertLargeXaml()
    {
        var loader = new XamlLoader();
        var document = loader.Load(v_LargeXamlPath);

        if (document.Root == null)
            throw new InvalidOperationException("No root element");

        IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        var ir = converter.Convert(document.Root);

        var renderer = HtmlRendererFactory.Create();
        var html = renderer.RenderDocument(ir);

        return html;
    }

    /// <summary>
    /// Benchmark: Just the XML to IR conversion phase
    /// 
    /// Isolates conversion performance from rendering.
    /// Useful for measuring parser efficiency independently.
    /// </summary>
    [Benchmark(Description = "Parsing Phase Only")]
    public object ParseOnly()
    {
        var loader = new XamlLoader();
        var document = loader.Load(v_SampleXamlPath);

        if (document.Root == null)
            throw new InvalidOperationException("No root element");

        IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        var ir = converter.Convert(document.Root);

        return ir;
    }

    /// <summary>
    /// Benchmark: Just the rendering phase
    /// 
    /// Measures rendering performance independently.
    /// Useful for detecting renderer bottlenecks.
    /// </summary>
    [Benchmark(Description = "Rendering Phase Only")]
    public string RenderOnly()
    {
        var loader = new XamlLoader();
        var document = loader.Load(v_SampleXamlPath);

        if (document.Root == null)
            throw new InvalidOperationException("No root element");

        IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        var ir = converter.Convert(document.Root);

        var renderer = HtmlRendererFactory.Create();
        return renderer.RenderDocument(ir);
    }
}
