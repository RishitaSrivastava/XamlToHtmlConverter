// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents a multi-condition data-bound trigger in the intermediate representation.
/// Applies property setters only when all specified binding conditions evaluate to their target values.
/// Equivalent to XAML's MultiDataTrigger element.
/// </summary>
public class IntermediateRepresentationMultiDataTrigger
{
    #region Public Properties

    /// <summary>
    /// Gets the collection of binding-based conditions that must all be true for the trigger to activate.
    /// Each condition pairs a binding path with the required value.
    /// </summary>
    public List<(string BindingPath, string Value)> Conditions { get; } = new();

    /// <summary>
    /// Gets the collection of property-value pairs to apply when all conditions are met.
    /// Maps WPF property names to their target values.
    /// </summary>
    public Dictionary<string, string> Setters { get; } = new();

    #endregion
}
