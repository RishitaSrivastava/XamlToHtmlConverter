// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Default implementation of IBindingAttributeBuilder.
/// Extracts data binding paths and trigger attributes from IR elements,
/// formatting them as HTML data-* attributes.
/// 
/// Binding normalization:
///   "IsSelected" → data-binding-selected
///   "IsChecked" → data-binding-checked
///   "IsEnabled" → data-binding-enabled
///   "Visibility" → data-binding-visibility
///   Custom → data-binding-{propertyname}
/// </summary>
public class DefaultBindingAttributeBuilder : IBindingAttributeBuilder
{
    /// <summary>
    /// Normalizes binding property names to HTML attribute-friendly names.
    /// </summary>
    private static readonly Dictionary<string, string> s_PropertyNormalization = new(StringComparer.OrdinalIgnoreCase)
    {
        { "IsSelected", "selected" },
        { "IsChecked", "checked" },
        { "IsEnabled", "enabled" },
        { "Visibility", "visibility" },
        { "Text", "text" },
        { "Content", "content" },
        { "SelectedItem", "selected-item" },
        { "ItemsSource", "items-source" },
        { "SelectedValue", "selected-value" }
    };

    /// <summary>
    /// Builds a dictionary of binding attributes from the IR element.
    /// </summary>
    public Dictionary<string, string> Build(IntermediateRepresentationElement element)
    {
        var result = new Dictionary<string, string>();

        // Extract data bindings
        foreach (var binding in element.Bindings)
        {
            if (string.IsNullOrWhiteSpace(binding.Value.Path))
                continue;

            var normalizedKey = NormalizeBindingKey(binding.Key);
            result[$"data-binding-{normalizedKey}"] = binding.Value.Path;
        }

        // Extract trigger constraints
        foreach (var trigger in element.Triggers)
        {
            if (string.IsNullOrWhiteSpace(trigger.Property))
                continue;

            var key = $"data-trigger-{trigger.Property.ToLower()}";
            result[key] = trigger.Value ?? "true";
        }

        // Extract multi-trigger constraints
        foreach (var multiTrigger in element.MultiTriggers)
        {
            if (multiTrigger.Conditions.Count == 0)
                continue;

            var key = "data-trigger-multi";
            result[key] = string.Join("|", multiTrigger.Conditions.Select(c => $"{c.Property}={c.Value}"));
        }

        return result;
    }

    /// <summary>
    /// Normalizes binding property names for use in HTML attributes.
    /// </summary>
    private static string NormalizeBindingKey(string key)
    {
        if (s_PropertyNormalization.TryGetValue(key, out var normalized))
            return normalized;

        // Fallback: convert camelCase to kebab-case
        return ConvertCamelCaseToKebabCase(key);
    }

    /// <summary>
    /// Converts camelCase property names to kebab-case for HTML attributes.
    /// Example: "IsSelected" → "is-selected"
    /// </summary>
    private static string ConvertCamelCaseToKebabCase(string input)
    {
        // Capacity optimized for binding attribute construction (typically 100-300 chars)
        var result = new System.Text.StringBuilder(255);
        for (int i = 0; i < input.Length; i++)
        {
            var ch = input[i];
            if (char.IsUpper(ch) && i > 0)
                result.Append('-');
            result.Append(char.ToLower(ch));
        }
        return result.ToString();
    }
}
