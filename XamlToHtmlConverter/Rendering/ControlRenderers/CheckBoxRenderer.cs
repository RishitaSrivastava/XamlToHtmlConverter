using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

public class CheckBoxRenderer : IControlRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "CheckBox";

    public void RenderAttributes(
    IntermediateRepresentationElement element,
    AttributeBuffer attributes)
    {
        attributes.Add("type", "checkbox");

        if (element.Properties.TryGetValue("IsChecked", out var value))
        {
            attributes.Add("data-binding-checked", value);
        }
    }
    public void RenderContent(
    IntermediateRepresentationElement element,
    StringBuilder sb,
    int indent,
    Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // CheckBox has no special content rendering
    }
}