// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

/// <summary>
/// Defines a contract for converting specific XAML properties
/// into equivalent CSS styles.
/// Implementations handle property-specific mapping logic (e.g., Width, Margin, Padding).
/// </summary>
public interface IPropertyMapper
{
    /// <summary>
    /// Determines whether this mapper can handle the specified XAML property name.
    /// </summary>
    /// <param name="propertyName">The name of the XAML property (e.g., "Width", "Margin").</param>
    /// <returns><c>true</c> if this mapper supports the property; otherwise, <c>false</c>.</returns>
    bool CanHandle(string propertyName);

    /// <summary>
    /// Converts the XAML property value to CSS and appends it to the style builder.
    /// </summary>
    /// <param name="element">The IR element containing the property.</param>
    /// <param name="propertyValue">The raw XAML property value string.</param>
    /// <param name="context">The layout context providing additional rendering information.</param>
    /// <param name="styleBuilder">The string builder to append CSS declarations to.</param>
    void Apply(
        IntermediateRepresentationElement element,
        string propertyValue,
        LayoutContext context,
        StringBuilder styleBuilder);
}   