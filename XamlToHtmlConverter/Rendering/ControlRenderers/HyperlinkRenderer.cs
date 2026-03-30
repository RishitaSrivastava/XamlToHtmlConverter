// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for Hyperlink elements.
/// Maps to <c>&lt;a href="#"&gt;</c>. When a NavigateUri property is present,
/// it is used as the href value instead of the placeholder.
/// </summary>
public class HyperlinkRenderer : IAttributeRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "Hyperlink";

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        var href = element.Properties.TryGetValue("NavigateUri", out var uri)
                   && !string.IsNullOrWhiteSpace(uri)
            ? uri
            : "#";

        attributes.Add("href", href);
    }
}
