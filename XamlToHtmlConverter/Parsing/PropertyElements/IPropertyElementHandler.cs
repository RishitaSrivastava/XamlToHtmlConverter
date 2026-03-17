// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements;

/// <summary>
/// Defines a contract for handling XAML property elements during XML-to-IR conversion.
/// Property elements are complex properties defined as child XML elements
/// (e.g., Grid.RowDefinitions, Control.Template, Resources).
/// </summary>
public interface IPropertyElementHandler
{
    /// <summary>
    /// Determines whether this handler can process the specified XML element.
    /// </summary>
    /// <param name="elementName">The local name of the XML element to evaluate.</param>
    /// <returns><c>true</c> if this handler supports the element; otherwise, <c>false</c>.</returns>
    bool CanHandle(string elementName);

    /// <summary>
    /// Processes the XML property element and populates the IR element accordingly.
    /// Extracts structured data (e.g., row/column definitions, styles, templates)
    /// and stores it in the appropriate IR properties.
    /// </summary>
    /// <param name="node">The XML element representing the property.</param>
    /// <param name="ir">The target IR element to populate.</param>
    /// <param name="convert">A callback function to recursively convert child XML elements to IR elements.</param>
    void Handle(
        XElement node,
        IntermediateRepresentationElement ir,
        Func<XElement, IntermediateRepresentationElement> convert);
}