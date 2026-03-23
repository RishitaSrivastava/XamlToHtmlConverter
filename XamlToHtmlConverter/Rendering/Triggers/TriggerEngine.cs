// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Processes XAML triggers and multi-triggers from IR elements
/// and generates corresponding HTML data attributes for client-side handling.
/// </summary>
public static class TriggerEngine
{
    #region Public Methods

    /// <summary>
    /// Extracts single-condition triggers from an IR element and converts them
    /// into HTML data attributes suitable for client-side JavaScript handling.
    /// </summary>
    /// <param name="element">The IR element containing triggers.</param>
    /// <returns>
    /// A dictionary of data attribute names (e.g., "data-trigger-ismouseover")
    /// mapped to their trigger definitions in the format "value:style".
    /// </returns>
    /// <remarks>
    /// Example output: data-trigger-ismouseover="True:Background:Blue;Foreground:White"
    /// </remarks>
    public static Dictionary<string, string> Extract(
        IntermediateRepresentationElement element)
    {
        var result = new Dictionary<string, string>();

        foreach (var trigger in element.Triggers)
        {
            var key = $"data-trigger-{trigger.Property.ToLower()}";

            var sb_style = new StringBuilder();
            bool first = true;
            foreach (var setter in trigger.Setters)
            {
                if (!first) sb_style.Append(";");
                sb_style.Append(setter.Key).Append(":").Append(setter.Value);
                first = false;
            }

            result[key] = $"{trigger.Value}:{sb_style}";
        }
        
        return result;
    }

    /// <summary>
    /// Extracts multi-condition triggers from an IR element and converts them
    /// into HTML data attributes for client-side processing.
    /// All conditions must be satisfied for the trigger to activate.
    /// </summary>
    /// <param name="element">The IR element containing multi-triggers.</param>
    /// <returns>
    /// A dictionary of data attribute names (e.g., "data-multitrigger")
    /// mapped to their condition and setter definitions.
    /// </returns>
    /// <remarks>
    /// Example output: data-multitrigger="IsMouseOver=True&IsEnabled=True:Background:Blue;Foreground:White"
    /// </remarks>
    public static Dictionary<string, string> ExtractMultiTriggers(
        IntermediateRepresentationElement element)
    {
        var result = new Dictionary<string, string>();

        foreach (var trigger in element.MultiTriggers)
        {
            var sbCondition = new StringBuilder();
            bool firstCond = true;
            foreach (var c in trigger.Conditions)
            {
                if (!firstCond) sbCondition.Append("&");
                sbCondition.Append(c.Property).Append("=").Append(c.Value);
                firstCond = false;
            }

            var sbSetter = new StringBuilder();
            bool firstSetter = true;
            foreach (var s in trigger.Setters)
            {
                if (!firstSetter) sbSetter.Append(";");
                sbSetter.Append(s.Key).Append(":").Append(s.Value);
                firstSetter = false;
            }

            result[$"data-multitrigger"] = $"{sbCondition}:{sbSetter}";
        }

        return result;
    }   

    #endregion
}