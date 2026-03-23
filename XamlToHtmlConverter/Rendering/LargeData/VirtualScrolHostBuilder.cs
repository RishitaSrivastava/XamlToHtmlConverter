using System.Text;
using XamlToHtmlConverter.Rendering;

namespace XamlToHtmlConverter.Rendering.LargeData;

/// <summary>
/// Builder for virtual scroll host HTML structure.
/// Provides consistent indent handling via HtmlRenderer.
/// </summary>
public static class VirtualScrollHostBuilder
{
    /// <summary>
    /// Builds the virtual scroll host container HTML.
    /// </summary>
    /// <param name="sb">The string builder to append to.</param>
    /// <param name="indent">The indentation level.</param>
    public static void Build(StringBuilder sb, int indent)
    {
        var spacing = new string(' ', indent);
        sb.AppendLine($"{spacing}<div class=\"virtual-scroll-host\">");
        sb.AppendLine($"{spacing}  <div class=\"virtual-scroll-content\"></div>");
        sb.AppendLine($"{spacing}</div>");
    }
}
