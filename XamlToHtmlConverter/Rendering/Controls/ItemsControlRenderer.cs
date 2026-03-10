// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.ControlRenderers;

namespace XamlToHtmlConverter.Rendering.Controls
{
    public class ItemsControlRenderer : IControlRenderer
    {
        public bool CanHandle(IntermediateRepresentationElement element)
        {
            return element.Type == "ItemsControl";
        }

        public void RenderAttributes(
            IntermediateRepresentationElement element,
            StringBuilder sb)
        {
            if (element.Type == "ListBox")
            {
                sb.Append(" multiple");
            }

            if (element.Properties.TryGetValue("ItemsSource", out var source))
            {
                sb.Append($" data-itemssource=\"{source}\"");
            }
        }

        public void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild)
            {
                // Default ItemsControl behavior handled elsewhere
            }
    }
}