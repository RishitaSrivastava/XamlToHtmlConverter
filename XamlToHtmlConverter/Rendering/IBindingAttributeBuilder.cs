// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Defines a contract for extracting and formatting data binding attributes
/// from IR elements. Extracted from IStyleBuilder to satisfy Single 
/// Responsibility Principle.
/// </summary>
public interface IBindingAttributeBuilder
{
    /// <summary>
    /// Builds a dictionary of HTML data-* attributes representing binding paths
    /// and trigger constraints from the IR element.
    /// </summary>
    /// <param name="element">The intermediate representation element to extract bindings from.</param>
    /// <returns>A dictionary mapping HTML attribute names to binding/trigger values.</returns>
    Dictionary<string, string> Build(IntermediateRepresentationElement element);
}
