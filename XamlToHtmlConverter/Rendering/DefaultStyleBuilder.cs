// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Default implementation of <see cref="IStyleBuilder"/>.
    /// Generates inline CSS styles from standard element properties,
    /// attached layout properties, and parent layout context.
    /// </summary>
    public class DefaultStyleBuilder : IStyleBuilder
    {
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
            ApplyStandardProperties(element, sb);
            ApplyAttachedProperties(element, sb);
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

            foreach (var prop in element.Properties)
            {
                var value = prop.Value.Trim();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (value.StartsWith("{Binding") && value.EndsWith("}"))
                {
                    var inner = value.Substring(8, value.Length - 9).Trim();
                    string? path = null;

                    if (inner.Contains("Path=", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = inner.Split(',');
                        foreach (var part in parts)
                        {
                            var trimmed = part.Trim();
                            if (trimmed.StartsWith("Path=", StringComparison.OrdinalIgnoreCase))
                            {
                                path = trimmed.Substring(5).Trim();
                                break;
                            }
                        }
                    }
                    else
                    {
                        path = inner.Split(',')[0].Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(path))
                        result[$"data-binding-{prop.Key.ToLower()}"] = path;
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
        private void ApplyStandardProperties(IntermediateRepresentationElement element, StringBuilder sb)
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
        }

        /// <summary>
        /// Appends grid row, column, span, and z-index CSS positioning properties
        /// derived from the element's attached XAML properties.
        /// </summary>
        /// <param name="element">The IR element to read attached properties from.</param>
        /// <param name="sb">The string builder to append CSS to.</param>
        private void ApplyAttachedProperties(IntermediateRepresentationElement element, StringBuilder sb)
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
        }

        /// <summary>
        /// Appends alignment CSS properties (justify-self, align-self) to the style builder
        /// based on the element's HorizontalAlignment and VerticalAlignment properties,
        /// adjusted according to the parent layout type and orientation.
        /// </summary>
        /// <param name="element">The IR element whose alignment properties are evaluated.</param>
        /// <param name="context">The layout context of the parent container.</param>
        /// <param name="sb">The string builder to append CSS to.</param>
        private void ApplyAlignment(IntermediateRepresentationElement element, LayoutContext context, StringBuilder sb)
        {
            if (string.Equals(context.ParentLayoutType, "Grid", StringComparison.OrdinalIgnoreCase))
            {
                if (element.Properties.TryGetValue("HorizontalAlignment", out var hAlign))
                {
                    var css = ConvertAlignment(hAlign);
                    if (css != null)
                        sb.Append($"justify-self:{css};");
                }

                if (element.Properties.TryGetValue("VerticalAlignment", out var vAlign))
                {
                    var css = ConvertAlignment(vAlign);
                    if (css != null)
                        sb.Append($"align-self:{css};");
                }
                return;
            }

            if (string.Equals(context.ParentLayoutType, "StackPanel", StringComparison.OrdinalIgnoreCase))
            {
                var orientation = context.ParentOrientation ?? "Vertical";

                if (string.Equals(orientation, "Vertical", StringComparison.OrdinalIgnoreCase))
                {
                    if (element.Properties.TryGetValue("HorizontalAlignment", out var hAlign))
                    {
                        var css = ConvertAlignment(hAlign);
                        if (css != null)
                            sb.Append($"align-self:{css};");
                    }
                }
                else
                {
                    if (element.Properties.TryGetValue("VerticalAlignment", out var vAlign))
                    {
                        var css = ConvertAlignment(vAlign);
                        if (css != null)
                            sb.Append($"align-self:{css};");
                    }
                }
            }
        }

        /// <summary>
        /// Converts a XAML alignment value into its equivalent CSS flexbox/grid self-alignment keyword.
        /// Returns <c>null</c> for Stretch since that is the default and requires no explicit CSS.
        /// </summary>
        /// <param name="value">The XAML alignment value (e.g., Left, Right, Center, Stretch).</param>
        /// <returns>The CSS keyword (e.g., start, end, center), or <c>null</c> for Stretch or unrecognized values.</returns>
        private string? ConvertAlignment(string value)
        {
            return value switch
            {
                "Left"    => "start",
                "Right"   => "end",
                "Top"     => "start",
                "Bottom"  => "end",
                "Center"  => "center",
                _         => null
            };
        }

        /// <summary>
        /// Converts a XAML Thickness value into a CSS spacing shorthand string.
        /// Supports single, two-value (left,top), and four-value (left,top,right,bottom) formats.
        /// </summary>
        /// <param name="thickness">The raw XAML Thickness string (e.g., "10", "10,5", "10,5,10,5").</param>
        /// <returns>The equivalent CSS margin or padding value string (e.g., "10px", "5px 10px").</returns>
        private string ConvertThickness(string thickness)
        {
            var parts = thickness.Split(',');

            if (parts.Length == 1)
                return $"{parts[0]}px";

            if (parts.Length == 2)
                return $"{parts[1]}px {parts[0]}px";

            if (parts.Length == 4)
            {
                var left   = parts[0];
                var top    = parts[1];
                var right  = parts[2];
                var bottom = parts[3];
                return $"{top}px {right}px {bottom}px {left}px";
            }

            return thickness;
        }

        #endregion
    }
}
