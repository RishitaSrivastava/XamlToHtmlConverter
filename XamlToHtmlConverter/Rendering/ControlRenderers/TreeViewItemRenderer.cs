// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for TreeViewItem elements.
/// - Adds <c>role="treeitem"</c> to each <c>&lt;li&gt;</c> for accessibility.
/// - Leaf nodes: emits Header text inline — <c>&lt;li role="treeitem"&gt;Text&lt;/li&gt;</c>.
/// - Branch nodes: uses <c>&lt;details&gt;&lt;summary&gt;</c> for native expand/collapse
///   without any JavaScript.
/// </summary>
public class TreeViewItemRenderer : IContentRenderer, IAttributeRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "TreeViewItem";

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        attributes.Add("role", "treeitem");
    }

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        element.Properties.TryGetValue("Header", out var header);
        var encodedHeader = System.Net.WebUtility.HtmlEncode(header ?? string.Empty);

        var treeChildren = element.Children
            .Where(c => c.Type == "TreeViewItem")
            .ToList();

        if (treeChildren.Count == 0)
        {
            // Leaf: emit text inline so </li> immediately follows.
            sb.Append(encodedHeader);
        }
        else
        {
            // Branch: use <details><summary> for native expand/collapse.
            var ind2 = new string(' ', indent + 2);
            var ind4 = new string(' ', indent + 4);
            sb.AppendLine();
            sb.AppendLine($"{ind2}<details>");
            sb.AppendLine($"{ind4}<summary>{encodedHeader}</summary>");
            sb.AppendLine($"{ind4}<ul role=\"group\">");
            foreach (var child in treeChildren)
                renderChild(child, sb, indent + 6);
            sb.AppendLine($"{ind4}</ul>");
            sb.AppendLine($"{ind2}</details>");
            sb.Append(new string(' ', indent));
        }
    }
}

