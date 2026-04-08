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

    /// <summary>
    /// Gets or sets a cached flag indicating whether the condition can be expressed as a CSS pseudo-class.
    /// Set during first rendering pass and reused for subsequent renders.
    /// </summary>
    /// <remarks>
    /// Performance optimization: Avoids repeated TriggerCssPropertyMapper lookups.
    /// Tri-state: null (not yet evaluated), true (maps to CSS), false (cannot use CSS).
    /// </remarks>
    public bool? CachedCanUseCssRule { get; set; }

    /// <summary>
    /// Gets or sets the pre-computed CSS pseudo-class (e.g., ":hover", ":not(:disabled)").
    /// Populated during first render pass and reused for subsequent renders.
    /// </summary>
    /// <remarks>
    /// Performance optimization: Eliminates dictionary lookup on every render.
    /// Only valid when CachedCanUseCssRule is true.
    /// </remarks>
    public string? CachedCssPseudoClass { get; set; }

    #endregion
}