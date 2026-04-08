// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for TreeView elements.
/// Adds <c>role="tree"</c> to the outer <c>&lt;ul&gt;</c> for accessibility.
/// </summary>
public class TreeViewRenderer : IAttributeRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "TreeView";

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        attributes.Add("role", "tree");
    }
}
