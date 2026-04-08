// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Handles multi-condition property triggers (<see cref="IntermediateRepresentationMultiTrigger"/>).
/// 
/// <para>When every condition in a MultiTrigger maps to a CSS pseudo-class, the conditions are
/// combined into a compound CSS selector (e.g., <c>:hover:not(:disabled)</c>) and a single
/// CSS rule is emitted.</para>
/// <para>If any condition cannot be expressed in CSS, the trigger falls back to an indexed
/// <c>data-multitrigger-{index}</c> attribute that the JS runtime evaluates.
/// Using indexed keys avoids the key-collision bug present in the original implementation.</para>
/// </summary>
public sealed class MultiTriggerHandler : ITriggerHandler
{
    /// <inheritdoc />
    public void Process(
        IntermediateRepresentationElement element,
        string elementSelector,
        TriggerOutput output)
    {
        for (int i = 0; i < element.MultiTriggers.Count; i++)
        {
            var trigger = element.MultiTriggers[i];

            if (TryBuildCssRule(trigger, elementSelector, out var cssRule))
                output.CssRules.Add(cssRule!);
            // If not all conditions map to CSS pseudo-classes, drop silently.
            // Never emit data-multitrigger-* attributes.
        }
    }

    private static bool TryBuildCssRule(
        IntermediateRepresentationMultiTrigger trigger,
        string selector,
        out string? cssRule)
    {
        cssRule = null;

        if (trigger.Conditions.Count == 0 || trigger.Setters.Count == 0)
            return false;

        // OPTIMIZATION: Check cache for previously evaluated result
        // Avoids repeated TriggerCssPropertyMapper lookups for triggers processed multiple times
        if (trigger.CachedCanUseCssRule.HasValue)
        {
            if (!trigger.CachedCanUseCssRule.Value)
                return false;

            // Cached as CSS-compatible: rebuild rule from cached pseudo-class suffix
            var cachedDecl = BuildCssDeclarations(trigger.Setters);
            cssRule = $"{selector}{trigger.CachedCombinedPseudoClass} {{ {cachedDecl} }}";
            return true;
        }

        // Cache miss: evaluate conditions and cache result
        var pseudoSuffixes = new List<string>(trigger.Conditions.Count);

        foreach (var condition in trigger.Conditions)
        {
            if (!TriggerCssPropertyMapper.TryGetCssPseudoClass(
                    condition.Property, condition.Value, out var pseudo))
            {
                // Cache negative result: not all conditions map to CSS
                trigger.CachedCanUseCssRule = false;
                return false;
            }

            pseudoSuffixes.Add(pseudo);
        }

        // Cache combined pseudo-classes for future renders
        var combinedPseudo = string.Concat(pseudoSuffixes);
        trigger.CachedCombinedPseudoClass = combinedPseudo;
        trigger.CachedCanUseCssRule = true;

        var cssDecl = BuildCssDeclarations(trigger.Setters);

        cssRule = $"{selector}{combinedPseudo} {{ {cssDecl} }}";
        return true;
    }

    private static string BuildCssDeclarations(Dictionary<string, string> setters)
    {
        // Capacity optimized for multi-trigger CSS declarations (typically 100-200 chars)
        var sb = new StringBuilder(255);

        foreach (var setter in setters)
        {
            var (cssProp, cssVal) = TriggerCssPropertyMapper.MapSetterToCss(setter.Key, setter.Value);
            sb.Append(cssProp).Append(':').Append(cssVal).Append(';');
        }

        return sb.ToString();
    }

    private static string SerializeConditions(List<(string Property, string Value)> conditions)
    {
        // Capacity optimized for condition serialization (small, ~50-100 chars)
        var sb = new StringBuilder(127);
        bool first = true;

        foreach (var c in conditions)
        {
            if (!first) sb.Append('&');
            sb.Append(c.Property).Append('=').Append(c.Value);
            first = false;
        }

        return sb.ToString();
    }

    private static string SerializeSetters(Dictionary<string, string> setters)
    {
        // Capacity optimized for setter serialization (medium, ~100-200 chars)
        var sb = new StringBuilder(255);
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
