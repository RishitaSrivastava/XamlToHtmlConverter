// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for ProgressBar elements, mapping to HTML <progress value="X" max="Y">.
/// Emits the value and max attributes so the browser renders a filled progress bar.
/// </summary>
public class ProgressBarRenderer : IAttributeRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "ProgressBar";

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Value property → HTML value attribute
        if (element.Properties.TryGetValue("Value", out var value))
            attributes.Add("value", value);

        // Maximum property → HTML max attribute (default 100 if not specified)
        var max = element.Properties.TryGetValue("Maximum", out var maxVal) ? maxVal : "100";
        attributes.Add("max", max);

        // Minimum is implicitly 0 in HTML <progress>; no min attribute needed
    }
}
