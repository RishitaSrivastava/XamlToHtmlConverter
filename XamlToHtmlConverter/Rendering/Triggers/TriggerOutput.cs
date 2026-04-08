// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Holds the results of processing all triggers on a single IR element.
/// Distinguishes between triggers that can be expressed as pure CSS rules
/// and those that require runtime evaluation via data attributes and JavaScript.
/// </summary>
public class TriggerOutput
{
    #region Public Properties

    /// <summary>
    /// Gets the list of complete CSS rule strings generated from triggers that
    /// map to CSS pseudo-classes (e.g., :hover, :active, :focus, :disabled, :checked).
    /// Each rule is a fully-formed CSS declaration block including a selector.
    /// Example: "#myButton:hover { background-color: blue; color: white; }"
    /// </summary>
    public List<string> CssRules { get; } = new();

    /// <summary>
    /// Gets the dictionary of HTML data attributes for triggers that require
    /// runtime JavaScript evaluation (data-trigger-*, data-datatrigger-*, etc.).
    /// Keys are attribute names; values encode trigger conditions and setters.
    /// </summary>
    public Dictionary<string, string> DataAttributes { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether any trigger on this element requires
    /// the inline JavaScript runtime to be emitted into the HTML document.
    /// Set to true when DataTriggers, MultiDataTriggers, or EventTrigger actions are present.
    /// </summary>
    public bool RequiresJsRuntime { get; set; }

    #endregion
}
