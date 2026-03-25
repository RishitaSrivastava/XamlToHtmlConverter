using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for TextBox elements.
/// Handles attribute rendering (type, value, validation) and content rendering (empty).
/// Implements IAttributeRenderer to ensure input type and properties are always applied.
/// </summary>
public class TextBoxRenderer : IAttributeRenderer, IContentRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "TextBox";

    public void RenderAttributes(
        IntermediateRepresentationElement element,
        AttributeBuffer attributes)
    {
        bool acceptsReturn = element.Properties.TryGetValue("AcceptsReturn", out var ar)
            && ar.Equals("True", StringComparison.OrdinalIgnoreCase);

        // Set appropriate input type based on AcceptsReturn property
        if (!acceptsReturn)
            attributes.Add("type", "text");

        // Add value binding or initial text content
        if (element.Properties.TryGetValue("Text", out var text))
            attributes.Add("value", text);

        // Add HTML validation constraints
        if (element.Properties.TryGetValue("MaxLength", out var max))
            attributes.Add("maxlength", max);

        // Add readonly attribute if specified
        if (element.Properties.TryGetValue("IsReadOnly", out var ro)
            && ro.Equals("True", StringComparison.OrdinalIgnoreCase))
            attributes.Add("readonly", "");
    }

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // TextBox is self-closing, no content rendering
    }
}
