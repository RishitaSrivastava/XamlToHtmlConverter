// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Handles single-condition property triggers (<see cref="IntermediateRepresentationTrigger"/>).
///
/// <para>Only triggers whose property/value pair maps to a CSS pseudo-class
/// (e.g. IsMouseOver=True → :hover) produce output — a CSS rule is emitted.
/// Triggers that have no CSS equivalent (e.g. IsHighlighted) are silently dropped;
/// no <c>data-trigger-*</c> attributes or JS runtime hooks are emitted.
/// This follows the graceful-degradation policy: prefer correct HTML over emulated behaviour.</para>
/// </summary>
public sealed class PropertyTriggerHandler : ITriggerHandler
{
    /// <inheritdoc />
    public void Process(
        IntermediateRepresentationElement element,
        string elementSelector,
        TriggerOutput output)
    {
        foreach (var trigger in element.Triggers)
        {
            if (!TriggerCssPropertyMapper.TryGetCssPseudoClass(
                    trigger.Property, trigger.Value, out var pseudo))
                continue; // No CSS equivalent — drop silently.

            var cssDecl = BuildCssDeclarations(trigger.Setters);
            if (!string.IsNullOrWhiteSpace(cssDecl))
                output.CssRules.Add($"{elementSelector}{pseudo} {{ {cssDecl} }}");
        }
    }

    private static string BuildCssDeclarations(Dictionary<string, string> setters)
    {
        var sb = new StringBuilder();

        foreach (var setter in setters)
        {
            var (cssProp, cssVal) = TriggerCssPropertyMapper.MapSetterToCss(setter.Key, setter.Value);
            sb.Append(cssProp).Append(':').Append(cssVal).Append(';');
        }

        return sb.ToString();
    }

    private static string SerializeSetters(Dictionary<string, string> setters)
    {
        var sb = new StringBuilder();
        bool first = true;

        foreach (var s in setters)
        {
            if (!first) sb.Append(';');
            sb.Append(s.Key).Append(':').Append(s.Value);
            first = false;
        }

        return sb.ToString();
    }
}
