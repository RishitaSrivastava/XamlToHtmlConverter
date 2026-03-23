// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Default implementation of <see cref="IEventExtractor"/>.
/// Scans IR element properties for known XAML event handler names
/// and converts them into HTML data-event-* attributes.
/// </summary>
public class DefaultEventExtractor : IEventExtractor
{
    #region Private Data

    /// <summary>
    /// Holds the set of supported XAML event names that can be
    /// converted into HTML metadata attributes.
    /// </summary>
    private static readonly HashSet<string> v_KnownEvents = new()
    {
        "Click",
        "TextChanged",
        "Checked",
        "Unchecked",
        "Loaded",
        "SelectionChanged"
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Scans the element's properties for known event names
    /// and returns their corresponding data-event-* HTML attributes.
    /// </summary>
    /// <param name="element">The IR element to extract event attributes from.</param>
    /// <returns>
    /// A dictionary of HTML attribute names (e.g., data-event-click)
    /// mapped to their handler names.
    /// </returns>
    public Dictionary<string, string> Extract(IntermediateRepresentationElement element)
    {
        var result = new Dictionary<string, string>();

        foreach (var prop in element.Properties)
            if (v_KnownEvents.Contains(prop.Key))
                result[$"data-event-{prop.Key.ToLower()}"] = prop.Value;

        return result;
    }

    #endregion
}