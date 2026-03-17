// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents a multi-condition trigger in the intermediate representation.
/// Applies property setters only when all specified conditions are satisfied.
/// Equivalent to XAML's MultiTrigger element.
/// </summary>
public class IntermediateRepresentationMultiTrigger
{
    #region Public Properties

    /// <summary>
    /// Gets the collection of conditions that must all be true for the trigger to activate.
    /// Each condition consists of a property name and the required value.
    /// </summary>
    public List<(string Property, string Value)> Conditions { get; }
        = new();

    /// <summary>
    /// Gets the collection of property-value pairs to apply when all conditions are met.
    /// Maps property names to their target values.
    /// </summary>
    public Dictionary<string, string> Setters { get; }
        = new();

    #endregion
}