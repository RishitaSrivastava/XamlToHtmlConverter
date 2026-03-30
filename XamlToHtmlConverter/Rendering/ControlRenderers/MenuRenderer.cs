// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for Menu elements.
/// Emits <c>&lt;nav aria-label="Main menu"&gt;&lt;ul role="menubar"&gt;</c>
/// with each MenuItem wrapped as <c>&lt;li role="none"&gt;&lt;button role="menuitem"&gt;Text&lt;/button&gt;&lt;/li&gt;</c>
/// — a pattern that is valid HTML5, accessible, and keyboard-navigable.
/// </summary>
public class MenuRenderer : IContentRenderer, IAttributeRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "Menu";

    public void RenderAttributes(IntermediateRepresentationElement element, AttributeBuffer attributes)
    {
        attributes.Add("aria-label", "Main menu");
    }

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        var ind2 = new string(' ', indent + 2);
        var ind4 = new string(' ', indent + 4);
        var ind6 = new string(' ', indent + 6);

        sb.AppendLine();
        sb.AppendLine($"{ind2}<ul role=\"menubar\">");

        foreach (var item in element.Children)
        {
            item.Properties.TryGetValue("Header", out var label);
            var text = System.Net.WebUtility.HtmlEncode(label ?? string.Empty);
            sb.AppendLine($"{ind4}<li role=\"none\">");
            sb.AppendLine($"{ind6}<button role=\"menuitem\">{text}</button>");
            sb.AppendLine($"{ind4}</li>");
        }

        sb.AppendLine($"{ind2}</ul>");
        sb.Append(new string(' ', indent));
    }
}

