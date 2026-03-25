using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for CheckBox elements.
/// Handles both attribute rendering (type, binding) and content rendering (empty).
/// Implements IAttributeRenderer to ensure type="checkbox" is always applied.
/// </summary>
public class CheckBoxRenderer : IAttributeRenderer, IContentRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "CheckBox";

    public void RenderAttributes(
        IntermediateRepresentationElement element,
        AttributeBuffer attributes)
    {
        // Always add the HTML checkbox type
        attributes.Add("type", "checkbox");

        // Add binding for two-way data binding
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
        // CheckBox is self-closing, no content rendering
    }
}