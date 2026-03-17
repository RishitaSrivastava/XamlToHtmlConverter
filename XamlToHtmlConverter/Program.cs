// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Diagnostics;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.Rendering;
using XamlToHtmlConverter.Rendering.Behavior;
using XamlToHtmlConverter.Rendering.Behavior.Handlers;
using XamlToHtmlConverter.Rendering.ControlRenderers;
using XamlToHtmlConverter.Rendering.Controls;

namespace XamlToHtmlConverter;

/// <summary>
/// Entry point of the XAML-to-HTML converter application.
/// Coordinates XAML loading, IR conversion, XML export,
/// HTML rendering, and console inspection.
/// </summary>
internal class Program
{
    #region Private Methods

    /// <summary>
    /// Executes the end-to-end XAML to HTML conversion pipeline.
    /// Loads the XAML file, converts it to an IR tree, exports the IR as XML,
    /// renders the HTML output, and prints the IR structure to the console.
    /// Collects performance metrics for observability.
    /// </summary>
    private static void Main()
    {
        var totalWatch = Stopwatch.StartNew();

        var path = Path.Combine(AppContext.BaseDirectory, "sample.xaml");

        // Phase 1: Load XAML
        var loadWatch = Stopwatch.StartNew();
        var loader = new XamlLoader();
        var document = loader.Load(path);
        loadWatch.Stop();

        if (document.Root == null)
            throw new InvalidOperationException("XML document has no root element.");

        // Save original XML DOM
        var xmlOutputPath = Path.Combine(AppContext.BaseDirectory, "XamlDom.xml");
        document.Save(xmlOutputPath);

        // Phase 2: Convert to IR
        var conversionWatch = Stopwatch.StartNew();
        IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        var ir = converter.Convert(document.Root);
        conversionWatch.Stop();

        // Count elements for metrics
        var elementCount = CountElements(ir);

        // Save IR representation
        var irDoc = IntermediateRepresentationXmlExporter.Export(ir);
        var irOutputPath = Path.Combine(AppContext.BaseDirectory, "Ir.xml");
        irDoc.Save(irOutputPath);

        // Phase 3: Render HTML
        var renderingWatch = Stopwatch.StartNew();
        var renderer = HtmlRendererFactory.Create();
        var html = renderer.RenderDocument(ir);
        renderingWatch.Stop();

        var htmlOutputPath = Path.Combine(AppContext.BaseDirectory, "output.html");
        File.WriteAllText(htmlOutputPath, html);

        totalWatch.Stop();

        // Collect and display metrics
        var metrics = new PerformanceMetrics
        {
            LoadingTime = loadWatch.Elapsed,
            ConversionTime = conversionWatch.Elapsed,
            RenderingTime = renderingWatch.Elapsed,
            TotalTime = totalWatch.Elapsed,
            ElementCount = elementCount,
            StyleCount = CountStyles(html)
        };

        Console.WriteLine(metrics.ToString());

        // Print IR structure to console
        PrintIr(ir, 0);
        Console.ReadLine();
    }

    /// <summary>
    /// Recursively counts all elements in the IR tree.
    /// Used for metrics collection and scaling analysis.
    /// </summary>
    private static int CountElements(IntermediateRepresentationElement element)
    {
        int count = 1; // Count this element
        foreach (var child in element.Children)
            count += CountElements(child);
        return count;
    }

    /// <summary>
    /// Counts CSS classes in the rendered HTML output.
    /// Provides insight into style deduplication effectiveness.
    /// </summary>
    private static int CountStyles(string html)
    {
        // Count occurrences of class= to estimate CSS class usage
        return html.Split("class=").Length - 1;
    }

    /// <summary>
    /// Recursively prints the IR tree structure to the console,
    /// including regular properties, attached properties, inner text,
    /// control templates, and child elements.
    /// </summary>
    /// <param name="element">The IR element to print.</param>
    /// <param name="indent">The current indentation level in spaces.</param>
    private static void PrintIr(IntermediateRepresentationElement element, int indent)
    {
        var space = new string(' ', indent);
        foreach (var prop in element.Properties)
            Console.WriteLine($"{space}  prop: {prop.Key}={prop.Value}");

        foreach (var attached in element.AttachedProperties)
            Console.WriteLine($"{space}  Attached: {attached.Key}={attached.Value}");

        if (!string.IsNullOrWhiteSpace(element.InnerText))
            Console.WriteLine($"{space}  Text: {element.InnerText}");

        if (element.Template != null)
        {
            Console.WriteLine($"{space}  [Template]");
            PrintIr(element.Template, indent + 4);
        }

        foreach (var child in element.Children)
            PrintIr(child, indent + 2);
    }

    #endregion
}