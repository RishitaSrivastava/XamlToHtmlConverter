using System.Collections.Generic;
using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Parsing;

namespace XamlToHtmlConverter.Rendering.Behavior;

/// <summary>
/// Helper for extracting binding information from XAML binding expressions.
/// </summary>
internal static class BindingExpressionHelper
{
    /// <summary>
    /// Returns the binding path from a "{Binding ...}" expression,
    /// or allows behavior handlers to use this for data-binding attributes.
    /// Delegates to BindingParser to avoid reimplementing the same logic.
    /// </summary>
    public static string ExtractPath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var binding = BindingParser.Parse(value);
        if (binding != null && !string.IsNullOrWhiteSpace(binding.Path))
            return binding.Path;
        return value; // not a binding, return as-is (e.g. plain string "True")
    }
}