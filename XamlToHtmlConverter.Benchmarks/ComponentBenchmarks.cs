// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using BenchmarkDotNet.Attributes;
using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Benchmarks;

/// <summary>
/// Component-level benchmarks for isolated performance analysis.
/// Measures individual component bottlenecks without pipeline interference.
/// 
/// Components tested:
/// - BindingParser: XAML binding expression parsing
/// - StyleRegistry: CSS style deduplication
/// - ControlRendererRegistry: Renderer resolution
/// - XamlLoader: XML parsing and loading
/// </summary>
[SimpleJob(warmupCount: 3, targetCount: 5)]
[MemoryDiagnoser]
[RankColumn]
public class ComponentBenchmarks
{
    private string? v_SampleXamlPath;
    private XElement? v_TestElement;
    private StyleRegistry? v_StyleRegistry;

    [GlobalSetup]
    public void Setup()
    {
        var baseDir = AppContext.BaseDirectory;
        v_SampleXamlPath = Path.Combine(baseDir, "..", "..", "..", "XamlToHtmlConverter", "sample.xaml");
        
        // Create test element
        const string xamlNs = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        v_TestElement = new XElement(XName.Get("Button", xamlNs),
            new XAttribute("Content", "Click Me"),
            new XAttribute("Width", "100"),
            new XAttribute("Height", "50"));

        v_StyleRegistry = new StyleRegistry();
    }

    /// <summary>
    /// Benchmark: XAML binding expression parsing
    /// 
    /// Tests: BindingParser.Parse()
    /// Measures allocation efficiency when parsing binding expressions.
    /// Examples:
    /// - "{Binding UserName}"
    /// - "{Binding Path=Items, Mode=TwoWay}"
    /// </summary>
    [Benchmark(Description = "Simple Binding Parse")]
    public object? ParseSimpleBinding()
    {
        return BindingParser.Parse("{Binding UserName}");
    }

    /// <summary>
    /// Benchmark: Complex binding expression parsing
    /// 
    /// Tests multi-property binding parsing performance.
    /// </summary>
    [Benchmark(Description = "Complex Binding Parse")]
    public object? ParseComplexBinding()
    {
        return BindingParser.Parse("{Binding Path=Items, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}");
    }

    /// <summary>
    /// Benchmark: Style registration and deduplication
    /// 
    /// Tests: StyleRegistry.Register()
    /// Measures cache efficiency and style normalization.
    /// </summary>
    [Benchmark(Description = "Style Registration")]
    public string RegisterStyle()
    {
        const string style = "display: flex; flex-direction: column; gap: 10px; color: red;";
        return v_StyleRegistry!.Register(style);
    }

    /// <summary>
    /// Benchmark: Style deduplication (same style registered twice)
    /// 
    /// Measures cache hit performance.
    /// Should be much faster than first registration.
    /// </summary>
    [Benchmark(Description = "Style Cache Hit")]
    public string StyleCacheHit()
    {
        const string style = "padding: 10px; margin: 5px;";
        
        // Prime the cache
        v_StyleRegistry!.Register(style);
        
        // Cache hit - should be O(1)
        return v_StyleRegistry.Register(style);
    }

    /// <summary>
    /// Benchmark: XML to IR conversion (parser only)
    /// 
    /// Measures parsing performance without rendering.
    /// </summary>
    [Benchmark(Description = "Parser: XML to IR")]
    public object ParseXmlToIr()
    {
        IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        return converter.Convert(v_TestElement!);
    }

    /// <summary>
    /// Benchmark: XAML file loading (disk I/O + parsing)
    /// 
    /// Measures file I/O and XML parser performance.
    /// </summary>
    [Benchmark(Description = "XamlLoader: Load")]
    public object LoadXaml()
    {
        var loader = new XamlLoader();
        return loader.Load(v_SampleXamlPath!);
    }
}

