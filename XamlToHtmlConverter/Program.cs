// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter;

/// <summary>
/// Entry point of the XAML-to-HTML converter application.
/// Orchestrates the conversion pipeline and handles errors.
/// </summary>
internal class Program
{
    /// <summary>
    /// Main entry point. Executes the ConversionPipeline and prints IR structure to console.
    /// </summary>
    private static void Main()
    {
        try
        {
            var converter = new XmlToIrConverterRecursive();
            var renderer = HtmlRendererFactory.Create();
            var pipeline = new ConversionPipeline(converter, renderer);

            var inputPath = Path.Combine(AppContext.BaseDirectory, "sample2.xaml");
            var outputDirectory = AppContext.BaseDirectory;

            var metrics = pipeline.Run(inputPath, outputDirectory);
            Console.WriteLine(metrics.ToString());

            // Load IR for inspection
            var loader = new XamlLoader();
            var document = loader.Load(inputPath);
            if (document.Root != null)
            {
                var ir = converter.Convert(document.Root);
                Console.WriteLine("\n═══ IR STRUCTURE ═══════════════════════════");
                PrintIr(ir, 0);
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.Error.WriteLine($"Input file not found: {ex.FileName}");
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"Conversion failed: {ex.Message}");
            if (ex.InnerException != null)
                Console.Error.WriteLine($"Caused by: {ex.InnerException.Message}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected error: {ex}");
        }
        Console.ReadLine();
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
}