// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents a multi-condition trigger in the intermediate representation.
/// Applies property setters only when all specified conditions are satisfied.
/// Equivalent to XAML's MultiTrigger element.
/// 
/// OPTIMIZATION: Caches CSS pseudo-class mappings computed during parsing
/// to avoid repeated lookups during rendering. This eliminates the need to
/// call TriggerCssPropertyMapper for each condition during rendering.
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

    /// <summary>
    /// Gets the collection of pre-computed CSS pseudo-classes for each condition.
    /// Populated during parsing phase to avoid repeated mapper lookups during rendering.
    /// Index-aligned with Conditions list for efficient access.
    /// </summary>
    /// <remarks>
    /// Performance optimization: When this collection is non-empty, rendering can directly
    /// use pre-computed pseudo-classes instead of calling TriggerCssPropertyMapper for each
    /// condition. This eliminates dictionary lookups during the hot rendering path.
    /// </remarks>
    public List<string> CachedCssPseudoClasses { get; }
        = new();

    /// <summary>
    /// Gets or sets a cached flag indicating whether all conditions can be expressed as CSS pseudo-classes.
    /// Set during first rendering pass and reused for subsequent renders to avoid re-evaluation.
    /// </summary>
    /// <remarks>
    /// Performance optimization: Avoids repeated TriggerCssPropertyMapper lookups.
    /// Tri-state: null (not yet evaluated), true (all map to CSS), false (can't use CSS rule).
    /// </remarks>
    public bool? CachedCanUseCssRule { get; set; }

    /// <summary>
    /// Gets or sets the pre-computed combined CSS pseudo-class suffix (e.g., ":hover:not(:disabled)").
    /// Populated during first render pass and reused for subsequent renders.
    /// </summary>
    /// <remarks>
    /// Performance optimization: Eliminates string building on every render for CSS-compatible rules.
    /// Only valid when CachedCanUseCssRule is true.
    /// </remarks>
    public string? CachedCombinedPseudoClass { get; set; }

    #endregion
}