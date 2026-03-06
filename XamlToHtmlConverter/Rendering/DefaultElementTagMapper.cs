// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Default implementation of <see cref="IElementTagMapper"/>.
/// Maps known XAML element type names to their corresponding HTML tags.
/// Falls back to 'div' when no specific mapping is registered.
/// </summary>
public class DefaultElementTagMapper : IElementTagMapper
{
    #region Private Data

    /// <summary>
    /// Holds the mapping of XAML element type names to HTML tag names.
    /// </summary>
    private readonly Dictionary<string, string> v_TagMap = new()
    {
        { "Grid", "div" },
        { "StackPanel", "div" },
        { "Button", "button" },
        { "TextBlock", "span" },
        { "Border", "div" },
        { "CheckBox", "input" },
        { "RadioButton", "input" },
        { "Image", "img" },
        { "ComboBox", "select" },
        { "ListBox", "select" },
        { "ComboBoxItem", "option" },
        { "ListBoxItem", "option" },
        { "ContentControl", "div" },
        { "TextBox", "input" },
        { "WrapPanel", "div" }
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the HTML tag name that corresponds to the given XAML element type.
    /// Returns "div" as a fallback when no mapping is found.
    /// </summary>
    /// <param name="xamlType">The XAML element type name to look up.</param>
    /// <returns>The corresponding HTML tag name, or "div" if no mapping exists.</returns>
    public string Map(string xamlType)
    {
        if (v_TagMap.TryGetValue(xamlType, out var tag))
            return tag;

        return "div";
    }

    #endregion
}