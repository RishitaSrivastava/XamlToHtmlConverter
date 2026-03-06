// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents a style definition in the IR layer.
/// Stores metadata and property setters extracted from XAML style resources.
/// </summary>
public class IntermediateRepresentationStyle
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the unique key identifying the style within its resource dictionary.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the target element type to which the style applies.
    /// </summary>
    public string? TargetType { get; set; }

    /// <summary>
    /// Gets or sets the base style key reference for style inheritance scenarios.
    /// </summary>
    public string? BasedOn { get; set; }

    /// <summary>
    /// Gets the collection of property-value pairs defined as setters within the style.
    /// </summary>
    public Dictionary<string, string> Setters { get; } = new();

    #endregion
}