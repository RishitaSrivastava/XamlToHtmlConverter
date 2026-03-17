using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

public class TextBoxRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "TextBox";

    public void RenderAttributes(
    IntermediateRepresentationElement element,
    AttributeBuffer attributes)
    {
        attributes.Add("type", "text");

        if (element.Properties.TryGetValue("Text", out var value))
        {
            attributes.Add("value", value);
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