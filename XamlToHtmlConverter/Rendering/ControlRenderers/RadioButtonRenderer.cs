using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for RadioButton elements.
/// Handles both attribute rendering (type, name grouping, binding) and content rendering (empty).
/// Implements IAttributeRenderer to ensure type="radio" is always applied.
/// </summary>
public class RadioButtonRenderer : IAttributeRenderer, IContentRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "RadioButton";

    public void RenderAttributes(IntermediateRepresentationElement element,
        AttributeBuffer attributes)
    {
        // Always add HTML radio button type
        attributes.Add("type", "radio");

        // GroupName becomes HTML name attribute for grouping radio buttons
        if (element.Properties.TryGetValue("GroupName", out var group))
            attributes.Add("name", group);

        // Add binding for two-way data binding
        if (element.Bindings.TryGetValue("IsChecked", out var binding)
            && !string.IsNullOrEmpty(binding?.Path))
            attributes.Add("data-binding-checked", binding.Path!);
    }

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        // RadioButton is self-closing, no content rendering
    }
}