// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using BenchmarkDotNet.Attributes;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.Rendering;
using XamlToHtmlConverter.Rendering.StyleMappers;

namespace XamlToHtmlConverter.Benchmarks;

/// <summary>
/// Benchmarks comparing streaming vs regular HTML rendering.
/// Demonstrates memory and performance benefits of streaming approach.
/// 
/// Focus: Large document handling and memory efficiency
/// </summary>
[SimpleJob(warmupCount: 3, targetCount: 5)]
[MemoryDiagnoser]
[RankColumn]
public class StreamingRenderingBenchmarks
{
    private string? v_LargeXamlPath;
    private IXmlToIrConverter? v_Converter;
    private IStreamingHtmlRenderer? v_StreamingRenderer;
    private HtmlRenderer? v_RegularRenderer;

    [GlobalSetup]
    public void Setup()
    {
        var baseDir = AppContext.BaseDirectory;
        v_LargeXamlPath = Path.Combine(baseDir, "..", "..", "..", "XamlToHtmlConverter", "sample2.xaml");

        v_Converter = new XmlToIrConverterRecursive();
        v_RegularRenderer = HtmlRendererFactory.Create();
        
        var tagMapper = new DefaultElementTagMapper();
        
        // Provide default mappers (Dependency Inversion Principle)
        var mappers = new IPropertyMapper[]
        {
            new WidthMapper(),
            new HeightMapper(),
            new TypographyMapper(),
            new BorderMapper(),
            new PaddingMapper(),
            new MarginMapper(),
            new MinMaxSizeMapper(),
            new AlignmentMapper(),
            new TextAlignmentMapper()
        };
        
        var styleBuilder = new DefaultStyleBuilder(mappers);
        var layoutRenderers = new ILayoutRenderer[] { };
        v_StreamingRenderer = new StreamingHtmlRenderer(tagMapper, styleBuilder, layoutRenderers);
    }

    /// <summary>
    /// Benchmark: Regular rendering (full string buffering).
    /// Baseline: Everything held in memory until complete.
    /// </summary>
    [Benchmark(Description = "Regular Rendering (Full Buffer)")]
    public string RegularRendering()
    {
        var loader = new XamlLoader();
        var document = loader.Load(v_LargeXamlPath!);
        
        if (document.Root == null)
            throw new InvalidOperationException("No root");

        var ir = v_Converter!.Convert(document.Root);
        return v_RegularRenderer!.RenderDocument(ir);
    }

    /// <summary>
    /// Benchmark: Streaming rendering to file.
    /// Advantage: Memory stays constant regardless of output size.
    /// </summary>
    [Benchmark(Description = "Streaming Rendering (File Output)")]
    public string StreamingRenderingToFile()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var loader = new XamlLoader();
            var document = loader.Load(v_LargeXamlPath!);
            
            if (document.Root == null)
                throw new InvalidOperationException("No root");

            var ir = v_Converter!.Convert(document.Root);
            v_StreamingRenderer!.RenderToFile(ir, tempFile);
            
            // Return output for validation
            return File.ReadAllText(tempFile);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    /// <summary>
    /// Benchmark: Streaming rendering to string.
    /// Hybrid approach: Uses streaming internally but buffers final output.
    /// Note: Memory usage is similar to regular rendering in this case.
    /// </summary>
    [Benchmark(Description = "Streaming Rendering (String Buffer)")]
    public string StreamingRenderingToString()
    {
        var loader = new XamlLoader();
        var document = loader.Load(v_LargeXamlPath!);
        
        if (document.Root == null)
            throw new InvalidOperationException("No root");

        var ir = v_Converter!.Convert(document.Root);
        return v_StreamingRenderer!.RenderToString(ir);
    }
}

/// <summary>
/// Benchmarks for streaming pipeline end-to-end performance.
/// Measures complete conversion including all phases.
/// </summary>
[SimpleJob(warmupCount: 2, targetCount: 3)]
[MemoryDiagnoser]
public class StreamingPipelineBenchmarks
{
    private string? v_LargeXamlPath;
    private StreamingConversionPipeline? v_Pipeline;

    [GlobalSetup]
    public void Setup()
    {
        var baseDir = AppContext.BaseDirectory;
        v_LargeXamlPath = Path.Combine(baseDir, "..", "..", "..", "XamlToHtmlConverter", "sample2.xaml");

        var converter = new XmlToIrConverterRecursive();
        var tagMapper = new DefaultElementTagMapper();
        
        // Provide default mappers (Dependency Inversion Principle)
        var mappers = new IPropertyMapper[]
        {
            new WidthMapper(),
            new HeightMapper(),
            new TypographyMapper(),
            new BorderMapper(),
            new PaddingMapper(),
            new MarginMapper(),
            new MinMaxSizeMapper(),
            new AlignmentMapper(),
            new TextAlignmentMapper()
        };
        
        var styleBuilder = new DefaultStyleBuilder(mappers);
        var layoutRenderers = new ILayoutRenderer[] { };
        var renderer = new StreamingHtmlRenderer(tagMapper, styleBuilder, layoutRenderers);
        v_Pipeline = new StreamingConversionPipeline(converter, renderer);
    }

    /// <summary>
    /// End-to-end streaming conversion to file.
    /// Recommended approach for large documents.
    /// </summary>
    [Benchmark(Description = "End-to-End: XAML → File (Streaming)")]
    public StreamingConversionMetrics StreamingPipelineToFile()
    {
        var outputFile = Path.GetTempFileName();
        try
        {
            return v_Pipeline!.ConvertToFile(v_LargeXamlPath!, outputFile);
        }
        finally
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);
        }
    }

    /// <summary>
    /// End-to-end streaming conversion to string.
    /// Less memory-efficient but compatible with existing code.
    /// </summary>
    [Benchmark(Description = "End-to-End: XAML → String (Streaming)")]
    public StreamingConversionMetrics StreamingPipelineToString()
    {
        return v_Pipeline!.ConvertToString(v_LargeXamlPath!, out _);
    }
}
