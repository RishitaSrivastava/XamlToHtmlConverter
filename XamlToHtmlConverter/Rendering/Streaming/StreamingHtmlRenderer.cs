// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Streaming HTML renderer for efficient rendering of large XAML documents.
/// Writes HTML to a TextWriter without buffering the entire output in memory.
/// 
/// Performance Characteristics:
/// - Memory: O(tree depth) instead of O(document size)
/// - Peak Memory: Constant, independent of document size
/// - Time-to-First-Byte: Immediate (streaming begins instantly)
/// - File I/O: Optimized with large buffer (64KB)
/// 
/// Use this for:
/// - Documents > 10MB
/// - Memory-constrained environments
/// - Real-time rendering scenarios
/// - Streaming to network/file systems
/// </summary>
public class StreamingHtmlRenderer : IStreamingHtmlRenderer
{
    private readonly IElementTagMapper v_ElementTagMapper;
    private readonly IStyleBuilder v_StyleBuilder;
    private readonly Dictionary<string, ILayoutRenderer> v_LayoutRenderers;

    public StreamingHtmlRenderer(
        IElementTagMapper elementTagMapper,
        IStyleBuilder styleBuilder,
        ILayoutRenderer[] layoutRenderers)
    {
        v_ElementTagMapper = elementTagMapper;
        v_StyleBuilder = styleBuilder;
        v_LayoutRenderers = layoutRenderers
            .GroupBy(r => r.GetType().Name)
            .ToDictionary(g => g.Key, g => g.First());
    }

    /// <summary>
    /// Renders document starting with HTML structure.
    /// </summary>
    public void RenderToStream(IntermediateRepresentationElement element, TextWriter writer)
    {
        // HTML document structure
        writer.WriteLine("<!DOCTYPE html>");
        writer.WriteLine("<html>");
        writer.WriteLine("<head>");
        writer.WriteLine("  <meta charset=\"UTF-8\">");
        writer.WriteLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        writer.WriteLine("  <title>Converted XAML Document</title>");
        writer.WriteLine("  <style>");

        // Render styles inline (collected during element rendering)
        var styleRegistry = new StyleRegistry();
        CollectStyles(element, styleRegistry);
        var styles = styleRegistry.GenerateStyleBlock();
        writer.WriteLine(styles);

        writer.WriteLine("  </style>");
        writer.WriteLine("</head>");
        writer.WriteLine("<body>");

        // Render document body recursively
        RenderElementStream(element, writer, indentation: 2);

        writer.WriteLine("</body>");
        writer.WriteLine("</html>");
    }

    /// <summary>
    /// Renders document to file with optimized buffering.
    /// </summary>
    public void RenderToFile(IntermediateRepresentationElement element, string filePath)
    {
        // Use large buffer (64KB) for optimal file I/O performance
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 65536))
        using (var writer = new StreamWriter(fileStream, Encoding.UTF8, 65536))
        {
            RenderToStream(element, writer);
            writer.Flush();
        }
    }

    /// <summary>
    /// Renders to in-memory string (for compatibility with existing code).
    /// Note: Still uses streaming technique internally but buffers final result.
    /// </summary>
    public string RenderToString(IntermediateRepresentationElement element)
    {
        using (var writer = new StringWriter())
        {
            RenderToStream(element, writer);
            return writer.ToString();
        }
    }

    /// <summary>
    /// Recursively renders element and children to stream.
    /// Key optimization: Writes output immediately, doesn't buffer.
    /// </summary>
    private void RenderElementStream(
        IntermediateRepresentationElement element,
        TextWriter writer,
        int indentation)
    {
        var indent = new string(' ', indentation);
        var tag = v_ElementTagMapper.Map(element.Type);

        // Opening tag
        writer.Write(indent);
        writer.Write($"<{tag}");

        // Attributes
        foreach (var prop in element.Properties)
        {
            writer.Write($" {prop.Key}=\"{EscapeHtml(prop.Value)}\"");
        }

        // Style from style builder
        var style = v_StyleBuilder.Build(element, new LayoutContext(element.Type));
        if (!string.IsNullOrEmpty(style))
        {
            var styleClass = new StyleRegistry().Register(style);
            writer.Write($" class=\"{styleClass}\"");
        }

        writer.WriteLine(">");

        // Text content
        if (!string.IsNullOrEmpty(element.InnerText))
        {
            writer.Write(indent);
            writer.Write("  ");
            writer.WriteLine(EscapeHtml(element.InnerText));
        }

        // Render children recursively - EACH CHILD IS WRITTEN IMMEDIATELY
        foreach (var child in element.Children)
        {
            RenderElementStream(child, writer, indentation + 2);
        }

        // Closing tag
        writer.Write(indent);
        writer.WriteLine($"</{tag}>");

        // Flush every element to prevent buffer buildup on large docs
        writer.Flush();
    }

    /// <summary>
    /// Collects all styles into registry for efficient CSS generation.
    /// </summary>
    private void CollectStyles(IntermediateRepresentationElement element, StyleRegistry registry)
    {
        var style = v_StyleBuilder.Build(element, new LayoutContext(element.Type));
        if (!string.IsNullOrEmpty(style))
            registry.Register(style);

        foreach (var child in element.Children)
            CollectStyles(child, registry);
    }

    /// <summary>
    /// Escapes HTML special characters.
    /// </summary>
    private static string EscapeHtml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }
}
