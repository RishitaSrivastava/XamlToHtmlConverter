// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System;
using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.StyleMappers;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Default implementation of <see cref="IStyleBuilder"/>.
    /// Generates inline CSS styles from standard element properties,
    /// attached layout properties, and parent layout context.
    /// </summary>
    public class DefaultStyleBuilder : IStyleBuilder
    {
        private readonly PropertyMapperEngine v_PropertyEngine;
        public DefaultStyleBuilder()
        {
            v_PropertyEngine = new PropertyMapperEngine(new IPropertyMapper[]
            {
        new WidthMapper(),
        new HeightMapper()
            });
        }

        #region Public Methods

        /// <summary>
        /// Builds the complete inline CSS style string for the given IR element
        /// by delegating to specialized style handlers for standard properties,
        /// attached properties, and alignment.
        /// </summary>
        /// <param name="element">The IR element to generate CSS styles for.</param>
        /// <param name="context">The layout context describing the parent container type and orientation.</param>
        /// <returns>A combined CSS style string ready for use as an HTML style or class attribute value.</returns>
        public string Build(IntermediateRepresentationElement element, LayoutContext context)
        {
            var sb = new StringBuilder();
            v_PropertyEngine.Apply(element, context, sb);
            ApplyAttachedProperties(element, context, sb);
            ApplyAlignment(element, context, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Extracts data binding expressions from the IR element's properties
        /// and returns them as HTML data-binding-* attribute key-value pairs.
        /// </summary>
        /// <param name="element">The IR element to scan for binding expressions.</param>
        /// <returns>
        /// A dictionary mapping HTML attribute names (e.g., data-binding-text)
        /// to the corresponding binding path strings.
        /// </returns>
        public Dictionary<string, string> ExtractBindingAttributes(IntermediateRepresentationElement element)
        {
            var result = new Dictionary<string, string>();

            foreach (var binding in element.Bindings)
            {
                var path = binding.Value.Path;

                if (!string.IsNullOrWhiteSpace(path))
                {
                    result[$"data-binding-{binding.Key.ToLower()}"] = path;
                }
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Appends width, height, min/max dimensions, background color, margin, and padding
        /// CSS properties derived from the element's standard XAML properties.
        /// Returns early with display:none when Visibility is Collapsed.
        /// </summary>
        /// <param name="element">The IR element to read properties from.</param>
        /// <param name="sb">The string builder to append CSS to.</param>
        private void ApplyStandardProperties(
    IntermediateRepresentationElement element,
    LayoutContext context,
    StringBuilder sb)
        {
            if (element.Properties.TryGetValue("Visibility", out var visibility))
            {
                switch (visibility)
                {
                    case "Collapsed":
                        sb.Append("display:none;");
                        return;
                    case "Hidden":
                        sb.Append("visibility:hidden;");
                        break;
                    case "Visible":
                        break;
                }
            }

            if (element.Properties.TryGetValue("Width", out var width) && int.TryParse(width, out var w))
                sb.Append($"width:{w}px;");

            if (element.Properties.TryGetValue("Height", out var height) && int.TryParse(height, out var h))
                sb.Append($"height:{h}px;");

            if (element.Properties.TryGetValue("MinWidth", out var minWidth) && int.TryParse(minWidth, out var minW))
                sb.Append($"min-width:{minW}px;");

            if (element.Properties.TryGetValue("MaxWidth", out var maxWidth) && int.TryParse(maxWidth, out var maxW))
                sb.Append($"max-width:{maxW}px;");

            if (element.Properties.TryGetValue("MinHeight", out var minHeight) && int.TryParse(minHeight, out var minH))
                sb.Append($"min-height:{minH}px;");

            if (element.Properties.TryGetValue("MaxHeight", out var maxHeight) && int.TryParse(maxHeight, out var maxH))
                sb.Append($"max-height:{maxH}px;");

            if (element.Properties.TryGetValue("Background", out var bg))
                sb.Append($"background-color:{bg};");

            if (element.Properties.TryGetValue("Margin", out var margin))
                sb.Append($"margin:{ConvertThickness(margin)};");

            if (element.Properties.TryGetValue("Padding", out var padding))
                sb.Append($"padding:{ConvertThickness(padding)};");

            // WrapPanel item sizing support
            if (context.ParentLayoutType == "WrapPanel")
            {
                sb.Append("width:var(--wrap-item-width,auto);");
                sb.Append("height:var(--wrap-item-height,auto);");
            }
        }

        /// <summary>
        /// Appends grid row, column, span, and z-index CSS positioning properties
        /// derived from the element's attached XAML properties.
        /// </summary>
        /// <param name="element">The IR element to read attached properties from.</param>
        /// <param name="sb">The string builder to append CSS to.</param>
        private void ApplyAttachedProperties(IntermediateRepresentationElement element, LayoutContext context, StringBuilder sb)
        {
            // Grid row positioning
            if (element.AttachedProperties.TryGetValue("Grid.RowSpan", out var rowSpan)
                && int.TryParse(rowSpan, out var rs)
                && element.AttachedProperties.TryGetValue("Grid.Row", out var baseRow)
                && int.TryParse(baseRow, out var r))
            {
                sb.Append($"grid-row:{r + 1} / span {rs};");
            }
            else if (element.AttachedProperties.TryGetValue("Grid.Row", out var row)
                     && int.TryParse(row, out var rr))
            {
                sb.Append($"grid-row:{rr + 1};");
            }

            // Grid column positioning
            if (element.AttachedProperties.TryGetValue("Grid.ColumnSpan", out var colSpan)
                && int.TryParse(colSpan, out var cs)
                && element.AttachedProperties.TryGetValue("Grid.Column", out var baseCol)
                && int.TryParse(baseCol, out var c))
            {
                sb.Append($"grid-column:{c + 1} / span {cs};");
            }
            else if (element.AttachedProperties.TryGetValue("Grid.Column", out var col)
                     && int.TryParse(col, out var cc))
            {
                sb.Append($"grid-column:{cc + 1};");
            }

            // Z-index
            if (element.AttachedProperties.TryGetValue("Panel.ZIndex", out var zIndex)
                && int.TryParse(zIndex, out var z))
            {
                sb.Append($"z-index:{z};");
            }

            // DockPanel positioning
            if (element.AttachedProperties.TryGetValue("DockPanel.Dock", out var dock))
            {
                switch (dock)
                {
                    case "Top":
                        sb.Append("align-self:stretch;");
                        break;

                    case "Bottom":
                        sb.Append("align-self:stretch;margin-top:auto;");
                        break;

                    case "Left":
                        sb.Append("align-self:flex-start;");
                        break;

                    case "Right":
                        sb.Append("align-self:flex-end;");
                        break;
                }
            }
            // LastChildFill behavior
            if (context.ParentLayoutType == "DockPanel")
            {
                sb.Append("flex:1;");
            }
        }

        /// <summary>
        /// Appends alignment CSS properties (justify-self, align-self) to the style builder
        /// based on the element's HorizontalAlignment and VerticalAlignment properties,
        /// adjusted according to the parent layout type and orientation.
        /// </summary>
        /// <param name="element">The IR element whose alignment properties are evaluated.</param>
        /// <param name="context">The layout context of the parent container.</param>
        /// <param name="sb">The string builder to append CSS to.</param>
        private void ApplyAlignment(
    IntermediateRepresentationElement element,
    LayoutContext context,
    StringBuilder styleBuilder)
        {
            if (context.ParentLayoutType == null)
                return;

            element.Properties.TryGetValue("HorizontalAlignment", out var hAlign);
            element.Properties.TryGetValue("VerticalAlignment", out var vAlign);

            // GRID ALIGNMENT
            if (context.ParentLayoutType == "Grid")
            {
                styleBuilder.Append($"justify-self:{ConvertAlignment(hAlign)};");
                styleBuilder.Append($"align-self:{ConvertAlignment(vAlign)};");
                return;
            }

            // STACKPANEL
            if (context.ParentLayoutType == "StackPanel")
            {
                var orientation = context.ParentOrientation ?? "Vertical";

                if (orientation == "Vertical")
                {
                    // Cross axis = horizontal
                    if (!string.IsNullOrWhiteSpace(hAlign))
                        styleBuilder.Append($"align-self:{ConvertAlignment(hAlign)};");
                }
                else
                {
                    // Cross axis = vertical
                    if (!string.IsNullOrWhiteSpace(vAlign))
                        styleBuilder.Append($"align-self:{ConvertAlignment(vAlign)};");
                }

                return;
            }

            // WRAPPANEL
            if (context.ParentLayoutType == "WrapPanel")
            {
                if (!string.IsNullOrWhiteSpace(hAlign))
                    styleBuilder.Append($"align-self:{ConvertAlignment(hAlign)};");

                if (!string.IsNullOrWhiteSpace(vAlign))
                    styleBuilder.Append($"align-self:{ConvertAlignment(vAlign)};");
            }
        }

        /// <summary>
        /// Converts a XAML alignment value into its equivalent CSS flexbox/grid self-alignment keyword.
        /// Returns <c>null</c> for Stretch since that is the default and requires no explicit CSS.
        /// </summary>
        /// <param name="value">The XAML alignment value (e.g., Left, Right, Center, Stretch).</param>
        /// <returns>The CSS keyword (e.g., start, end, center), or <c>null</c> for Stretch or unrecognized values.</returns>
        private string ConvertAlignment(string? alignment)
        {
            if (string.IsNullOrWhiteSpace(alignment))
                return "stretch";

            return alignment switch
            {
                "Left" => "start",
                "Top" => "start",
                "Right" => "end",
                "Bottom" => "end",
                "Center" => "center",
                "Stretch" => "stretch",
                _ => "stretch"
            };
        }

        /// <summary>
        /// Converts a XAML Thickness value into a CSS spacing shorthand string.
        /// Supports single, two-value (left,top), and four-value (left,top,right,bottom) formats.
        /// </summary>
        /// <param name="thickness">The raw XAML Thickness string (e.g., "10", "10,5", "10,5,10,5").</param>
        /// <returns>The equivalent CSS margin or padding value string (e.g., "10px", "5px 10px").</returns>
        private string ConvertThickness(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "0";

            var parts = value.Split(',');

            if (parts.Length == 1)
            {
                if (int.TryParse(parts[0], out var all))
                    return $"{all}px";
            }

            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out var vertical) &&
                    int.TryParse(parts[1], out var horizontal))
                {
                    return $"{vertical}px {horizontal}px";
                }
            }

            if (parts.Length == 4)
            {
                var values = new List<string>();

                foreach (var p in parts)
                {
                    if (int.TryParse(p, out var px))
                        values.Add($"{px}px");
                }

                if (values.Count == 4)
                    return string.Join(" ", values);
            }

            return value;
        }

        #endregion
    }
}
