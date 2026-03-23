// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents a single-condition trigger in the intermediate representation.
/// Applies property setters when a specific property matches a specified value.
/// Equivalent to XAML's Trigger element.
/// </summary>
public class IntermediateRepresentationTrigger
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the name of the property to monitor for trigger activation.
    /// Example: "IsMouseOver", "IsEnabled".
    /// </summary>
    public string Property { get; set; } = "";

    /// <summary>
    /// Gets or sets the value that, when matched, activates the trigger.
    /// Example: "True", "False", "Red".
    /// </summary>
    public string Value { get; set; } = "";

    /// <summary>
    /// Gets the collection of property-value pairs to apply when the trigger condition is met.
    /// Maps property names to their target values.
    /// </summary>
    public Dictionary<string, string> Setters { get; } = new();

    #endregion
}