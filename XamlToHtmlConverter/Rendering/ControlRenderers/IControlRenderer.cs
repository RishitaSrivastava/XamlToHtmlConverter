// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Defines a contract for rendering specific control types with custom HTML generation logic.
/// Implementations provide specialized rendering for controls like TextBox, CheckBox, and ListBox
/// that require non-standard HTML structure or attributes.
/// </summary>
public interface IControlRenderer
{
    /// <summary>
    /// Determines whether this renderer can handle the specified IR element.
    /// </summary>
    /// <param name="element">The IR element to evaluate.</param>
    /// <returns><c>true</c> if this renderer supports the element type; otherwise, <c>false</c>.</returns>
    bool CanHandle(IntermediateRepresentationElement element);

    /// <summary>
    /// Generates control-specific HTML attributes and adds them to the attribute buffer.
    /// Used to emit specialized attributes like "type", "checked", "value", etc.
    /// </summary>
    /// <param name="element">The IR element being rendered.</param>
    /// <param name="attributes">The attribute buffer to populate with HTML attributes.</param>
    void RenderAttributes(
        IntermediateRepresentationElement element,
        AttributeBuffer attributes);

    /// <summary>
    /// Generates the inner HTML content for the control.
    /// Allows custom rendering of child elements or content that differs from the default behavior.
    /// </summary>
    /// <param name="element">The IR element being rendered.</param>
    /// <param name="sb">The string builder to append HTML content to.</param>
    /// <param name="indent">The current indentation level for formatting.</param>
    /// <param name="renderChild">A callback function to render child elements recursively.</param>
    void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild);
}