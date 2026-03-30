// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System;
using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.StyleMappers;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Default implementation of <see cref="IStyleBuilder"/>.
    /// Generates inline CSS styles from standard element properties,
    /// attached layout properties, and parent layout context.
    /// 
    /// Satisfies Dependency Inversion Principle:
    ///   - Property mappers are injected through constructor
    ///   - Allows substitution and testing of mapper implementations
    ///   - Factory provides default set of mappers
    /// </summary>
    public class DefaultStyleBuilder : IStyleBuilder
    {
        private readonly PropertyMapperEngine v_PropertyEngine;
        private readonly Dictionary<string, string> v_StyleRegistry = new();
        private int v_StyleCounter = 0;

        /// <summary>
        /// Initializes with the specified property mappers.
        /// Enables dependency injection and extension (D and O principles).
        /// </summary>
        /// <param name="mappers">The collection of property mappers to use.</param>
        public DefaultStyleBuilder(IEnumerable<IPropertyMapper> mappers)
        {
            v_PropertyEngine = new PropertyMapperEngine(mappers);
        }

        #region Public Methods

        /// <summary>
        /// Registers a CSS style string and returns a reusable class name.
        /// </summary>
        /// <param name="cssStyle">The CSS style string to register.</param>
        /// <returns>A class name that can be applied to HTML elements.</returns>
        public string Register(string cssStyle)
        {
            if (string.IsNullOrWhiteSpace(cssStyle))
                return string.Empty;

            var className = $"style-{v_StyleCounter++}";
            v_StyleRegistry[className] = cssStyle;
            return className;
        }

        /// <summary>
        /// Generates the complete &lt;style&gt; block for the HTML &lt;head&gt;.
        /// </summary>
        /// <returns>The consolidated CSS style block.</returns>
        public string GenerateStyleBlock()
        {
            if (v_StyleRegistry.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("<style>");

            foreach (var kvp in v_StyleRegistry)
            {
                sb.AppendLine($"  .{kvp.Key} {{ {kvp.Value} }}");
            }

            sb.AppendLine("</style>");
            return sb.ToString();
        }

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

            // Canvas container needs position:relative so absolute children are anchored to it
            if (element.Type == "Canvas")
                sb.Append("position:relative;");

            // Separator: vertical line in a horizontal flex row, horizontal line otherwise
            if (element.Type == "Separator")
            {
                if (context.ParentOrientation?.Equals("Horizontal", StringComparison.OrdinalIgnoreCase) == true)
                    sb.Append("align-self:stretch;border:none;border-left:1px solid currentColor;height:auto;margin:0 6px;width:0;");
                else
                    sb.Append("border:none;border-top:1px solid currentColor;margin:4px 0;width:100%;");
            }
            v_PropertyEngine.Apply(element, context, sb);
            ApplyAttachedProperties(element, context, sb);
            //ApplyAlignment(element, context, sb);
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
                                var key = binding.Key;

                                if (key == "IsSelected")
                                    key = "selected";
                                else if (key == "IsChecked")
                                    key = "checked";
                                else if (key == "IsEnabled")
                                    key = "enabled";
                                else if (key == "Visibility")
                                    key = "visibility";
                                else
                                    key = key.ToLower();

                                result[$"data-binding-{key}"] = path;
                }
            }

            return result;
        }

        #endregion

        #region Private Methods

        
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

            // Canvas absolute positioning
            bool hasCanvasAttached = element.AttachedProperties.ContainsKey("Canvas.Left")
                                  || element.AttachedProperties.ContainsKey("Canvas.Top");
            if (hasCanvasAttached)
                sb.Append("position:absolute;");

            if (element.AttachedProperties.TryGetValue("Canvas.Left", out var canvasLeft)
                && double.TryParse(canvasLeft, out var cl))
                sb.Append($"left:{cl}px;");

            if (element.AttachedProperties.TryGetValue("Canvas.Top", out var canvasTop)
                && double.TryParse(canvasTop, out var ct))
                sb.Append($"top:{ct}px;");

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
                        sb.Append("width:100%;");
                        sb.Append("order:-1;");
                        break;

                    case "Bottom":
                        sb.Append("align-self:stretch;");
                        sb.Append("width:100%;");
                        sb.Append("order:2;");
                        break;

                    case "Left":
                        sb.Append("display:flex;");
                        sb.Append("flex-direction:column;");
                        sb.Append("flex:0 0 auto;");
                        break;

                    case "Right":
                        sb.Append("display:flex;");
                        sb.Append("flex-direction:column;");
                        sb.Append("flex:0 0 auto;");
                        sb.Append("margin-left:auto;");
                        break;
                }
            }
            // LastChildFill behavior
            if (context.ParentLayoutType == "DockPanel")
            {
                if (!element.AttachedProperties.ContainsKey("DockPanel.Dock"))
                {
                    sb.Append("flex:1;");
                    
                }
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
        /// Supports single, two-value, and four-value formats in both space-separated 
        /// (XAML: "0 5") and comma-separated (internal: "0,5") formats.
        /// </summary>
        /// <param name="thickness">The raw XAML Thickness string (e.g., "10", "0 5", "0,5", "10,5,10,5").</param>
        /// <returns>The equivalent CSS margin or padding value string (e.g., "10px", "5px 10px").</returns>
        private string ConvertThickness(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "0";

            // Parse both space-separated (XAML format "0 5") and comma-separated ("0,5") formats
            var parts = value.Contains(',') 
                ? value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Trim whitespace from each part
            parts = parts.Select(p => p.Trim()).ToArray();

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
                {
                    var sb_thickness = new StringBuilder();
                    for (int i = 0; i < values.Count; i++)
                    {
                        if (i > 0) sb_thickness.Append(" ");
                        sb_thickness.Append(values[i]);
                    }
                    return sb_thickness.ToString();
                }
            }

            return value;
        }

        #endregion
    }
}
