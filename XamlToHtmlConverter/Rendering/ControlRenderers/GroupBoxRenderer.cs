// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for GroupBox elements, mapping to HTML fieldset with injected legend.
/// </summary>
public class GroupBoxRenderer : IContentRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "GroupBox";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        var childIndent = new string(' ', indent + 2);

        // Newline after <fieldset> opening tag, then <legend> on its own line
        sb.AppendLine();
        if (element.Properties.TryGetValue("Header", out var header))
            sb.AppendLine($"{childIndent}<legend>{System.Net.WebUtility.HtmlEncode(header)}</legend>");

        foreach (var child in element.Children)
            renderChild(child, sb, indent + 2);

        // Position </fieldset> at the same indent level as <fieldset>
        sb.Append(new string(' ', indent));
    }

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // GroupBox doesn't need special attributes beyond semantic fieldset
        // All styling should be handled by the style builder
    }
}
