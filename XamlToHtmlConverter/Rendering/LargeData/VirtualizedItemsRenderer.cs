using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.LargeData;

namespace XamlToHtmlConverter.Rendering.LargeData;

public class VirtualizedItemsRenderer
{
    public static void RenderPlaceholder(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent)
    {
        var spacing = new string(' ', indent);

        sb.AppendLine($"{spacing}<!-- Virtualized ItemsSource: {element.Bindings["ItemsSource"].Path} -->");

        VirtualScrollHostBuilder.Build(sb, indent);
    }
}