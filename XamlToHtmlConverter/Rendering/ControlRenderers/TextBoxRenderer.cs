using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

public class TextBoxRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "TextBox";

    public void RenderAttributes(
        IntermediateRepresentationElement element,
        StringBuilder sb)
    {
        sb.Append(" type=\"text\"");

        if (element.Properties.TryGetValue("Text", out var text))
        {
            var trimmed = text.Trim();

            if (!trimmed.StartsWith("{Binding"))
            {
                sb.Append($" value=\"{text}\"");
            }
        }
    }
    public void RenderContent(
    IntermediateRepresentationElement element,
    StringBuilder sb,
    int indent,
    Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // TextBox has no template or child rendering
    }
}