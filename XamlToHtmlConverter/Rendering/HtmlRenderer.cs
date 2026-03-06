// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Responsible for rendering an IR element tree into a complete HTML document.
    /// Orchestrates tag mapping, layout CSS generation, style deduplication,
    /// binding attribute emission, and event attribute emission.
    /// </summary>
    public class HtmlRenderer
    {
        #region Private Data

        /// <summary>
        /// Holds the mapper used to convert XAML element type names to HTML tag names.
        /// </summary>
        private readonly IElementTagMapper v_TagMapper;

        /// <summary>
        /// Holds the collection of layout renderers responsible for container layout behavior.
        /// </summary>
        private readonly IEnumerable<ILayoutRenderer> v_LayoutRenderers;

        /// <summary>
        /// Holds the style builder used to generate inline CSS styles for elements.
        /// </summary>
        private readonly IStyleBuilder v_StyleBuilder;

        /// <summary>
        /// Holds the event extractor used to retrieve event handler attributes from elements.
        /// </summary>
        private readonly IEventExtractor v_EventExtractor;

        /// <summary>
        /// Holds the style registry used to deduplicate inline CSS styles into reusable classes.
        /// </summary>
        private readonly StyleRegistry v_StyleRegistry = new();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the renderer with required mapping, layout, styling, and event services.
        /// </summary>
        /// <param name="tagMapper">The mapper for converting XAML types to HTML tags.</param>
        /// <param name="layoutRenderers">The collection of layout-specific CSS renderers.</param>
        /// <param name="styleBuilder">The builder for generating inline CSS from element properties.</param>
        /// <param name="eventExtractor">The extractor for converting XAML event handlers to HTML attributes.</param>
        public HtmlRenderer(
            IElementTagMapper tagMapper,
            IEnumerable<ILayoutRenderer> layoutRenderers,
            IStyleBuilder styleBuilder,
            IEventExtractor eventExtractor)
        {
            v_TagMapper = tagMapper;
            v_LayoutRenderers = layoutRenderers;
            v_StyleBuilder = styleBuilder;
            v_EventExtractor = eventExtractor;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates a full HTML document from the root IR element.
        /// Wraps the rendered body content with standard HTML boilerplate
        /// and a consolidated CSS style block.
        /// </summary>
        /// <param name="root">The root IR element representing the XAML tree.</param>
        /// <returns>A complete HTML document string.</returns>
        public string RenderDocument(IntermediateRepresentationElement root)
        {
            var bodyBuilder = new StringBuilder();
            RenderElement(root, bodyBuilder, 0, null, null);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\" />");
            sb.AppendLine("<title>XAML to HTML Output</title>");
            sb.AppendLine(v_StyleRegistry.GenerateStyleBlock());
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.Append(bodyBuilder.ToString());
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recursively renders an IR element and its children into HTML markup
        /// with appropriate indentation, CSS class assignment, binding attributes,
        /// event attributes, and special handling for input-type elements.
        /// </summary>
        /// <param name="element">The IR element to render.</param>
        /// <param name="sb">The string builder to append the rendered HTML to.</param>
        /// <param name="indent">The current indentation level in spaces.</param>
        /// <param name="parentLayoutType">The layout type name of the parent container, or null if none.</param>
        /// <param name="parentOrientation">The orientation of the parent container, or null if not applicable.</param>
        private void RenderElement(IntermediateRepresentationElement element, StringBuilder sb, int indent, string? parentLayoutType, string? parentOrientation)
        {
            var indentation = new string(' ', indent);
            var tag = v_TagMapper.Map(element.Type);
            var style = BuildStyle(element, parentLayoutType, parentOrientation);

            sb.Append($"{indentation}<{tag}");
            if (element.Type == "ListBox")
            {
                sb.Append(" multiple");
            }

            // Emit data-binding-* attributes
            var bindingAttributes = v_StyleBuilder.ExtractBindingAttributes(element);

            foreach (var attr in bindingAttributes)
            {
                sb.Append($" {attr.Key}=\"{attr.Value}\"");
            }

            // Emit data-event-* attributes
            var eventAttributes = v_EventExtractor.Extract(element);
            foreach (var evt in eventAttributes)
            {
                sb.Append($" {evt.Key}=\"{evt.Value}\"");
            }
            // ---- TextBox special handling ----
            if (element.Type == "TextBox")
            {
                sb.Append(" type=\"text\"");

                if (element.Properties.TryGetValue("Text", out var text))
                {
                    var trimmed = text.Trim();

                    bool isBinding =
                        trimmed.StartsWith("{Binding") &&
                        trimmed.EndsWith("}");

                    if (!isBinding)
                    {
                        sb.Append($" value=\"{text}\"");
                    }
                }
            }
            // CheckBox handling
            else if (element.Type == "CheckBox")
            {
                sb.Append(" type=\"checkbox\"");

                if (element.Properties.TryGetValue("IsChecked", out var isChecked) &&
                    string.Equals(isChecked, "True", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" checked");
                }
            }
            // RadioButton handling
            else if (element.Type == "RadioButton")
            {
                sb.Append(" type=\"radio\"");

                if (element.Properties.TryGetValue("IsChecked", out var isChecked) &&
                    string.Equals(isChecked, "True", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" checked");
                }
            }

            // Apply CSS class (deduplicated style)
            if (!string.IsNullOrWhiteSpace(style))
            {
                var className = v_StyleRegistry.Register(style);
                sb.Append($" class=\"{className}\"");
            }

            // Self-closing elements (input, img)
            if (tag == "input" || tag == "img")
            {
                if (element.Type == "Image" &&
                    element.Properties.TryGetValue("Source", out var src))
                {
                    sb.Append($" src=\"{src}\"");
                }

                sb.Append(" />");

                // Render content for CheckBox or RadioButton
                if ((element.Type == "CheckBox" || element.Type == "RadioButton") &&element.Properties.TryGetValue("Content", out var contentValue))
                {
                    sb.Append($" {contentValue}");
                }

                sb.AppendLine();
                return;
            }

            sb.Append(">");

            

            var content = ResolveElementContent(element);

            if (!string.IsNullOrWhiteSpace(content))
            {
                sb.Append(content);
            }


            // Render children recursively
            if (element.Children.Count > 0)
            {
                sb.AppendLine();
                for (int i = 0; i < element.Children.Count; i++)
                {
                    var child = element.Children[i];

                    string? orientation = null;

                    if (element.Type == "StackPanel" &&
                        element.Properties.TryGetValue("Orientation", out var o))
                    {
                        orientation = o;
                    }

                    // TextBlock ? TextBox ? label
                    if (child.Type == "TextBlock" &&
                        i + 1 < element.Children.Count &&
                        element.Children[i + 1].Type == "TextBox")
                    {
                        var labelText = child.InnerText ?? "";

                        sb.AppendLine($"{new string(' ', indent + 2)}<label>{labelText}</label>");

                        continue;
                    }

                    // Flatten ItemsControl.Items
                    if (child.Type == "ItemsControl.Items")
                    {
                        foreach (var item in child.Children)
                        {
                            RenderElement(item, sb, indent + 2, element.Type, orientation);
                        }
                    }

                    // Handle ItemTemplate
                    else if (child.Type == "ItemsControl.ItemTemplate")
                    {
                        foreach (var templateNode in child.Children)
                        {
                            if (templateNode.Type == "DataTemplate")
                            {
                                sb.AppendLine($"{new string(' ', indent + 2)}<template>");

                                foreach (var templateChild in templateNode.Children)
                                {
                                    RenderElement(templateChild, sb, indent + 4, element.Type, orientation);
                                }

                                sb.AppendLine($"{new string(' ', indent + 2)}</template>");
                            }
                        }
                    }

                    else
                    {
                        RenderElement(child, sb, indent + 2, element.Type, orientation);
                    }
                }

                sb.Append(indentation);
            }

            sb.AppendLine($"</{tag}>");
        }

        /// <summary>
        /// Builds the full inline CSS style string for an IR element by applying
        /// applicable layout renderers first, then appending property-based styles.
        /// </summary>
        /// <param name="element">The IR element to build styles for.</param>
        /// <param name="parentLayoutType">The layout type name of the parent container, or null.</param>
        /// <param name="parentOrientation">The orientation of the parent container, or null.</param>
        /// <returns>A combined CSS style string for the element.</returns>
        private string BuildStyle(IntermediateRepresentationElement element, string? parentLayoutType, string? parentOrientation)
        {
            var sb = new StringBuilder();

            // Apply layout container behavior (Grid, StackPanel, DockPanel, etc.)
            foreach (var layout in v_LayoutRenderers)
            {
                if (layout.CanHandle(element))
                {
                    layout.ApplyLayout(element, sb);
                    break;
                }
            }

            // Apply property-based styling (width, margin, alignment, grid positioning, etc.)
            var context = new LayoutContext(parentLayoutType, parentOrientation);
            sb.Append(v_StyleBuilder.Build(element, context));

            return sb.ToString();
        }
        private static string? ResolveElementContent(IntermediateRepresentationElement element)
        {
            if (!string.IsNullOrWhiteSpace(element.InnerText))
                return element.InnerText;

            if (element.Properties.TryGetValue("Text", out var text))
                return text;

            if (element.Properties.TryGetValue("Content", out var content))
                return content;

            return null;
        }

        #endregion
    }
}
