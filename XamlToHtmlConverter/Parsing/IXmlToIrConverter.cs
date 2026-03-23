// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing;

/// <summary>
/// Defines a contract for converting an XML DOM element
/// into a neutral intermediate representation (IR) element.
/// </summary>
public interface IXmlToIrConverter
{
    /// <summary>
    /// Converts the provided XML element and its subtree
    /// into the corresponding <see cref="IntermediateRepresentationElement"/> tree.
    /// </summary>
    /// <param name="element">The root XML element to convert.</param>
    /// <returns>The root <see cref="IntermediateRepresentationElement"/> of the converted IR tree.</returns>
    IntermediateRepresentationElement Convert(XElement element);
}