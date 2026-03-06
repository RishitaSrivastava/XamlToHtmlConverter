// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Provides functionality to export an IR element tree
    /// back into an XML document structure for inspection or persistence.
    /// </summary>
    public static class IntermediateRepresentationXmlExporter
    {
        #region Public Methods

        /// <summary>
        /// Converts the root IR element and its subtree into an <see cref="XDocument"/>.
        /// </summary>
        /// <param name="root">The root IR element to export.</param>
        /// <returns>An <see cref="XDocument"/> representing the full IR tree.</returns>
        public static XDocument Export(IntermediateRepresentationElement root)
        {
            var rootElement = ConvertToXElement(root);
            return new XDocument(rootElement);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recursively converts an IR element into an <see cref="XElement"/>,
        /// mapping properties, attached properties, inner text, and children.
        /// </summary>
        /// <param name="element">The IR element to convert.</param>
        /// <returns>The resulting <see cref="XElement"/>.</returns>
        private static XElement ConvertToXElement(IntermediateRepresentationElement element)
        {
            var xElement = new XElement(element.Type);

            foreach (var prop in element.Properties)
                xElement.Add(new XAttribute(prop.Key, prop.Value));

            foreach (var attached in element.AttachedProperties)
                xElement.Add(new XAttribute(attached.Key, attached.Value));

            if (!string.IsNullOrWhiteSpace(element.InnerText))
                xElement.Add(new XText(element.InnerText));

            foreach (var child in element.Children)
                xElement.Add(ConvertToXElement(child));

            return xElement;
        }

        #endregion
    }
}
