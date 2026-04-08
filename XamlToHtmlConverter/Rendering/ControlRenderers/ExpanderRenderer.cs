// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for Expander elements, mapping to HTML details with injected summary.
/// </summary>
public class ExpanderRenderer : IContentRenderer, IAttributeRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "Expander";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        var childIndent = new string(' ', indent + 2);

        // Newline after <details> opening tag, then <summary> on its own line
        sb.AppendLine();
        if (element.Properties.TryGetValue("Header", out var header))
            sb.AppendLine($"{childIndent}<summary>{System.Net.WebUtility.HtmlEncode(header)}</summary>");

        foreach (var child in element.Children)
            renderChild(child, sb, indent + 2);

        // Position </details> at the same indent level as <details>
        sb.Append(new string(' ', indent));
    }

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        // Handle IsExpanded property by setting open attribute on details
        if (element.Properties.TryGetValue("IsExpanded", out var isExpanded)
            && isExpanded.Equals("True", StringComparison.OrdinalIgnoreCase))
            attributes.Add("open", "");
    }
}
