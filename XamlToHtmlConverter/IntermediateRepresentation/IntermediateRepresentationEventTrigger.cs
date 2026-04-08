// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents an event-based trigger in the intermediate representation.
/// Fires actions in response to a routed event being raised on an element.
/// Equivalent to XAML's EventTrigger element.
/// </summary>
public class IntermediateRepresentationEventTrigger
{
    #region Public Properties

    /// <summary>
    /// Gets or sets the fully-qualified routed event name that activates this trigger.
    /// Example: "Button.Click", "Mouse.MouseEnter", "UIElement.GotFocus".
    /// </summary>
    public string RoutedEvent { get; set; } = "";

    /// <summary>
    /// Gets the collection of actions to execute when the routed event is raised.
    /// </summary>
    public List<IntermediateRepresentationTriggerAction> Actions { get; } = new();

    #endregion
}
