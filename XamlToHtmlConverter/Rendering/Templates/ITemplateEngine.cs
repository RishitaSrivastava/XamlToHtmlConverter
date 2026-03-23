// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Templates;

/// <summary>
/// Defines a contract for expanding control templates within IR elements.
/// Enables dependency inversion and testability of HtmlRenderer.
/// 
/// Replaces the hardcoded TemplateEngine field initialization in HtmlRenderer
/// with injected dependency, satisfying Dependency Inversion Principle.
/// </summary>
public interface ITemplateEngine
{
    /// <summary>
    /// Expands any control templates associated with the given IR element.
    /// Modifies the element's properties and children in-place.
    /// </summary>
    /// <param name="element">The IR element whose templates should be expanded.</param>
    void Expand(IntermediateRepresentationElement element);
}
