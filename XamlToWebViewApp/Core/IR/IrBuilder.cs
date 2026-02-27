using System.Xml.Linq;

namespace XamlToWebViewApp.Core.IR
{
    public class IrBuilder
    {
        /// <summary>
        /// Converts XAML XElement into IR model recursively
        /// </summary>
        public IrElement Build(XElement element)
        {
            // Create IR object
            var ir = new IrElement
            {
                Type = element.Name.LocalName
            };

            // Copy attributes
            foreach (var attr in element.Attributes())
            {
                ir.Properties[attr.Name.LocalName] = attr.Value;
            }

            // Process children
            foreach (var child in element.Elements())
            {
                ir.Children.Add(Build(child));
            }

            return ir;
        }
    }
}