// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for RepeatButton elements.
/// Maps to a plain <c>&lt;button&gt;</c> with a <c>title</c> attribute documenting
/// that continuous-fire behaviour is not replicated in HTML without JavaScript.
/// </summary>
public class RepeatButtonRenderer : IAttributeRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "RepeatButton";

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Continuous-fire / repeat behaviour requires JavaScript and is not emulated.
        attributes.Add("title", "RepeatButton: continuous-fire not supported in static HTML");
    }
}
