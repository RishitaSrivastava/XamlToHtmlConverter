// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Defines a contract for mapping XAML element type names
    /// to their corresponding HTML tag names.
    /// </summary>
    public interface IElementTagMapper
    {
        /// <summary>
        /// Returns the HTML tag name associated with the given XAML element type.
        /// </summary>
        /// <param name="xamlType">The XAML element type name (e.g., Grid, Button, TextBlock).</param>
        /// <returns>The corresponding HTML tag name (e.g., div, button, span).</returns>
        string Map(string xamlType);
    }
}
