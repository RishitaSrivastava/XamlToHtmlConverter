// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for ToggleButton elements.
/// Maps to <c>&lt;button aria-pressed="false"&gt;</c> so screen readers understand
/// the element has a pressed/unpressed state, matching WPF toggle semantics.
/// </summary>
public class ToggleButtonRenderer : IAttributeRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "ToggleButton";

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Reflect initial IsChecked state into aria-pressed.
        var isChecked = element.Properties.TryGetValue("IsChecked", out var v)
            && v.Equals("True", StringComparison.OrdinalIgnoreCase);

        attributes.Add("aria-pressed", isChecked ? "true" : "false");
    }
}
