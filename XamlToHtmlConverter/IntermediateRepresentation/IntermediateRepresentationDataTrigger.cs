// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents a data-bound trigger in the intermediate representation.
/// Applies property setters when a binding expression evaluates to a specified value.
/// Equivalent to XAML's DataTrigger element.
/// </summary>
public class IntermediateRepresentationDataTrigger
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the binding path that is monitored for trigger activation.
    /// Extracted from the XAML Binding markup extension, e.g. "{Binding IsActive}" → "IsActive".
    /// </summary>
    public string BindingPath { get; set; } = "";

    /// <summary>
    /// Gets or sets the value that, when matched by the binding result, activates the trigger.
    /// Example: "True", "Active".
    /// </summary>
    public string Value { get; set; } = "";

    /// <summary>
    /// Gets the collection of property-value pairs to apply when the trigger condition is met.
    /// Maps WPF property names to their target values.
    /// </summary>
    public Dictionary<string, string> Setters { get; } = new();

    #endregion
}
