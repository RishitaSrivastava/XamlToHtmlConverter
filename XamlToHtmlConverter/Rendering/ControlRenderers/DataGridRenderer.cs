// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Renderer for DataGrid elements.
/// Maps to a valid HTML <c>&lt;table&gt;</c> with empty <c>&lt;thead&gt;</c> and
/// <c>&lt;tbody&gt;</c> sections. This is treated as a static data representation;
/// sorting, virtualization, and column definitions are not replicated without JavaScript.
/// </summary>
public class DataGridRenderer : IContentRenderer
{
    public bool CanHandle(IntermediateRepresentationElement element)
        => element.Type == "DataGrid";

    public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
    {
        var ind2 = new string(' ', indent + 2);

        sb.AppendLine();
        sb.AppendLine($"{ind2}<thead></thead>");
        sb.AppendLine($"{ind2}<tbody></tbody>");
        sb.Append(new string(' ', indent));
    }
}
