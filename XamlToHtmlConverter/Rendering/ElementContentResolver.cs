// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text.RegularExpressions;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Resolves text content from an IR element using a fallback strategy.
/// Extracted from HtmlRenderer to satisfy Single Responsibility Principle.
/// 
/// Resolution order:
///   1. InnerText property (direct text content)
///   2. "Text" property (TextBlock, TextBox convention)
///      - If Text is a static string, return as-is
///      - If Text is a binding expression like "{Binding PropertyName}", extract and return the path as placeholder
///   3. "Text" binding (if no Text property, check bindings for data-binding-text)
///      - Returns the binding path as placeholder text (e.g., "UserCount" from binding path)
///   4. "Content" property (Button, Label convention)
///   5. null (no content found)
/// 
/// Binding expressions like "{Binding UserCount}" or bindings render as placeholder text "UserCount"
/// so elements with only binding content are visible in the browser.
/// </summary>
public static class ElementContentResolver
{
    private static readonly Regex BindingExpressionPattern = new(@"^\{Binding\s+([^}]+)\}$", RegexOptions.Compiled);

    /// <summary>
    /// Resolves the text content for a given IR element.
    /// For binding expressions, extracts the binding path as placeholder text.
    /// </summary>
    /// <param name="element">The intermediate representation element.</param>
    /// <returns>The resolved text content (or binding path for bindings), or null if no content found.</returns>
    public static string? Resolve(IntermediateRepresentationElement element)
    {
        // Priority 1: Direct inner text
        if (!string.IsNullOrWhiteSpace(element.InnerText))
            return element.InnerText;

        // Priority 2: "Text" property (common in TextBlock, Label, etc.)
        if (element.Properties.TryGetValue("Text", out var text))
        {
            // If text is static (not a binding), return as-is
            if (!string.IsNullOrWhiteSpace(text) && !text.Contains("{"))
                return text;

            // If text is a binding expression like "{Binding UserCount}", extract "UserCount" as placeholder
            var bindingMatch = BindingExpressionPattern.Match(text ?? "");
            if (bindingMatch.Success)
            {
                return bindingMatch.Groups[1].Value; // Return "UserCount" from "{Binding UserCount}"
            }

            // If text exists but is empty or malformed binding, still return it (might be whitespace)
            if (!string.IsNullOrWhiteSpace(text))
                return text;
        }

        // Priority 3: "Text" binding (separate from properties) - use binding path as placeholder
        if (element.Bindings.TryGetValue("Text", out var textBinding) && 
            !string.IsNullOrWhiteSpace(textBinding?.Path))
        {
            return textBinding.Path; // Return the binding path as visible placeholder (e.g., "UserCount")
        }

        // Priority 4: "Content" property (common in Button, Label, etc.)
        if (element.Properties.TryGetValue("Content", out var content))
            return content;

        // No content found
        return null;
    }
}

