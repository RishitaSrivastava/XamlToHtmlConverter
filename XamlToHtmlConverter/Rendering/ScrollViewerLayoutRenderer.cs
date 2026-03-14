using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering;

public class ScrollViewerLayoutRenderer : ILayoutRenderer
{
    public int Priority => 90;

    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "ScrollViewer";

    public void ApplyLayout(
        IntermediateRepresentationElement element,
        StringBuilder sb)
    {
        sb.Append("overflow:auto;");
    }
}