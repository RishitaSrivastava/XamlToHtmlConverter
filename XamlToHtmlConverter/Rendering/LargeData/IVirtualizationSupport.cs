// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.LargeData;

/// <summary>
/// Defines a contract for virtualization support in large data scenarios.
/// Abstracts away static virtualization utility classes for dependency injection 
/// and testability, satisfying Dependency Inversion Principle.
/// 
/// Enables conditional inclusion of virtualization CSS and host rendering
/// based on document analysis.
/// </summary>
public interface IVirtualizationSupport
{
    /// <summary>
    /// Determines whether an IR element requires virtualization support
    /// (typically large lists or repeated data).
    /// </summary>
    /// <param name="element">The IR element to analyze.</param>
    /// <returns>True if virtualization is needed, false otherwise.</returns>
    bool RequiresVirtualization(IntermediateRepresentationElement element);

    /// <summary>
    /// Builds CSS rules necessary for virtualization support.
    /// </summary>
    /// <returns>A CSS string to be included in the HTML style block.</returns>
    string BuildCss();

    /// <summary>
    /// Renders the virtualization host wrapper elements to the specified StringBuilder.
    /// </summary>
    /// <param name="indent">The current indentation level in spaces.</param>
    /// <returns>A string containing HTML markup for the virtual scroll host.</returns>
    string RenderVirtualHost(int indent);
}
