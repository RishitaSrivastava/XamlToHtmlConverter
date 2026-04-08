// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents an action within an EventTrigger in the intermediate representation.
/// Captures the action type and its associated configuration properties.
/// Examples: BeginStoryboard, SoundPlayerAction, PopupControlService.
/// </summary>
public class IntermediateRepresentationTriggerAction
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the type of action to perform when the event trigger fires.
    /// Example: "BeginStoryboard", "SoundPlayerAction".
    /// </summary>
    public string ActionType { get; set; } = "";

    /// <summary>
    /// Gets the collection of configuration properties for this action.
    /// Maps property names to their values as parsed from XAML attributes.
    /// </summary>
    public Dictionary<string, string> Properties { get; } = new();

    #endregion
}
