// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing;

/// <summary>
/// Converts XML elements into IR elements using LINQ-based traversal.
/// Maps attributes, text content, and child elements recursively.
/// </summary>
public class XmlToIrConverterLinqStyle : IXmlToIrConverter
{
    #region Public Methods

    /// <summary>
    /// Transforms the provided <see cref="XElement"/> into an <see cref="IntermediateRepresentationElement"/>.
    /// Processes attributes, inner text, and child elements using LINQ queries.
    /// </summary>
    /// <param name="element">The XML element to convert.</param>
    /// <returns>The resulting <see cref="IntermediateRepresentationElement"/> with all mapped properties and children.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
    public IntermediateRepresentationElement Convert(XElement element)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));

        var ir = new IntermediateRepresentationElement(element.Name.LocalName);

        // Map non-namespace attributes to regular or attached IR properties
        var attributes = element.Attributes().Where(a => !a.IsNamespaceDeclaration);
        foreach (var attr in attributes)
        {
            var name = attr.Name.LocalName;
            if (name.Contains("."))
                ir.AttachedProperties[name] = attr.Value;
            else
                ir.Properties[name] = attr.Value;
        }

        // Extract, trim, and combine all non-empty text nodes into inner text
        var text = element.Nodes().OfType<XText>().Select(t => t.Value.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t));
        var combined = string.Join(" ", text);
        if (!string.IsNullOrWhiteSpace(combined))
            ir.InnerText = combined;

        // Recursively convert child XML elements into IR elements
        var visualChildren = new List<IntermediateRepresentationElement>();
        foreach (var child in element.Elements())
        {
            var childName = child.Name.LocalName;
            if (childName == "Grid.RowDefinitions")
            {
                foreach (var rowDef in child.Elements())
                {
                    var heightAttr = rowDef.Attribute("Height");
                    if (heightAttr != null)
                        ir.GridRowDefinitions.Add(heightAttr.Value);
                }

                continue;
            }

            if (childName == "Grid.ColumnDefinitions")
            {
                foreach (var colDef in child.Elements())
                {
                    var widthAttr = colDef.Attribute("Width");
                    if (widthAttr != null)
                        ir.GridColumnDefinitions.Add(widthAttr.Value);
                }

                continue;
            }

            visualChildren.Add(Convert(child));
        }

        ir.Children = visualChildren;
        return ir;
    }

    #endregion
}