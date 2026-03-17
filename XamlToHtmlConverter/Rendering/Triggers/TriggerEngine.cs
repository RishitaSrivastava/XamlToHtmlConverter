// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

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

            var style = string.Join(";",
                trigger.Setters.Select(s => $"{s.Key}:{s.Value}"));

            result[key] = $"{trigger.Value}:{style}";
        }
        
        Console.WriteLine($"Triggers found: {element.Triggers.Count}");
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
            var conditionString = string.Join("&",
                trigger.Conditions.Select(c =>
                    $"{c.Property}={c.Value}"));

            var setterString = string.Join(";",
                trigger.Setters.Select(s =>
                    $"{s.Key}:{s.Value}"));

            result[$"data-multitrigger"] =
                $"{conditionString}:{setterString}";
        }

        return result;
    }   

    #endregion
}