// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Resolves text content from an IR element using a fallback strategy.
/// Extracted from HtmlRenderer to satisfy Single Responsibility Principle.
/// 
/// Resolution order:
///   1. InnerText property (direct text content)
///   2. "Text" property (TextBlock, TextBox convention)
///   3. "Content" property (Button, Label convention)
///   4. null (no content found)
/// </summary>
public static class ElementContentResolver
{
    /// <summary>
    /// Resolves the text content for a given IR element.
    /// </summary>
    /// <param name="element">The intermediate representation element.</param>
    /// <returns>The resolved text content, or null if no content found.</returns>
    public static string? Resolve(IntermediateRepresentationElement element)
    {
        // Priority 1: Direct inner text
        if (!string.IsNullOrWhiteSpace(element.InnerText))
            return element.InnerText;

        // Priority 2: "Text" property (common in TextBlock, Label, etc.)
        if (element.Properties.TryGetValue("Text", out var text))
            return text;

        // Priority 3: "Content" property (common in Button, Label, etc.)
        if (element.Properties.TryGetValue("Content", out var content))
            return content;

        // No content found
        return null;
    }
}
