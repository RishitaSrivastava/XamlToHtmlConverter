// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Parsing;
using XamlToHtmlConverter.Rendering;
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
    /// </summary>
    private static void Main()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "sample.xaml");

        var loader = new XamlLoader();
        var document = loader.Load(path);
        if (document.Root == null)
            throw new InvalidOperationException("XML document has no root element.");

        // Save original XML DOM
        var xmlOutputPath = Path.Combine(AppContext.BaseDirectory, "XamlDom.xml");
        document.Save(xmlOutputPath);

        // Select conversion strategy
        Console.WriteLine("Recursive converter running...");
        IXmlToIrConverter converter = new XmlToIrConverterRecursive();
        var ir = converter.Convert(document.Root);
        PrintIr(ir, 0);

        // Save IR representation
        var irDoc = IntermediateRepresentationXmlExporter.Export(ir);
        var irOutputPath = Path.Combine(AppContext.BaseDirectory, "Ir.xml");
        irDoc.Save(irOutputPath);

        var layouts = new List<ILayoutRenderer>
            {
                new GridLayoutRenderer(),
                new StackPanelLayoutRenderer(),
                new DockPanelLayoutRenderer(),
                new WrapPanelLayoutRenderer()
            };
        var controlRegistry = new ControlRendererRegistry(new IControlRenderer[]
        {
            new TextBoxRenderer(),
            new CheckBoxRenderer(),
            new ItemsControlRenderer(),
            new ListBoxRenderer()

        });
        var renderer = new HtmlRenderer(
            new DefaultElementTagMapper(),
            layouts,
            new DefaultStyleBuilder(),
            new DefaultEventExtractor(),
            controlRegistry);

        var html = renderer.RenderDocument(ir);
        var htmlOutputPath = Path.Combine(AppContext.BaseDirectory, "output.html");
        File.WriteAllText(htmlOutputPath, html);

        // Print IR structure to console
        PrintIr(ir, 0);
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

    #endregion
}