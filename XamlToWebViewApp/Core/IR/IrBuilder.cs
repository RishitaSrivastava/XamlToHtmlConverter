using System.Xml.Linq;

namespace XamlToWebViewApp.Core.IR
{
    /// <summary>
    /// Builds an Intermediate Representation (IR) tree
    /// from parsed XAML XML nodes.
    ///
    /// Responsible for converting raw XAML structure into
    /// a renderer-independent UI model used by the
    /// HTML rendering pipeline.
    /// </summary>
    public class IrBuilder
    {
        /// <summary>
        /// Recursively converts an XElement into an IrElement.
        /// Copies element type, attributes, inner text,
        /// and child hierarchy.
        /// </summary>
        /// <param name="element">XAML XElement to convert.</param>
        /// <returns>Root IR element representing the XAML node.</returns>
        public IrElement Build(XElement element)
        {
            // Create IR element
            var ir = new IrElement
            {
                Type = element.Name.LocalName
            };

            // Copy attributes into IR properties
            foreach (var attr in element.Attributes())
            {
                ir.Properties[attr.Name.LocalName] = attr.Value;
            }

            // Capture inner text content (if any)
            // Example: <TextBlock>Hello</TextBlock>
            var textContent = element.Value?.Trim();

            if (!string.IsNullOrWhiteSpace(textContent)
                && !element.HasElements)
            {
                ir.InnerText = textContent;
            }

            // Recursively process child elements
            foreach (var child in element.Elements())
            {
                ir.Children.Add(Build(child));
            }

            return ir;
        }
    }
}