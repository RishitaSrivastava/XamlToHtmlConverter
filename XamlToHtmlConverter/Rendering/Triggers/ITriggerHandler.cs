// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Defines the contract for a single-responsibility trigger handler that processes
/// one category of XAML trigger (e.g., Trigger, DataTrigger, EventTrigger) and
/// writes its output into a shared <see cref="TriggerOutput"/>.
/// 
/// Follows the Open/Closed Principle — new trigger types are added as new
/// ITriggerHandler implementations without modifying existing code.
/// </summary>
public interface ITriggerHandler
{
    /// <summary>
    /// Processes all triggers of this handler's category on the given element
    /// and appends CSS rules and/or data attributes to <paramref name="output"/>.
    /// </summary>
    /// <param name="element">The IR element whose triggers should be processed.</param>
    /// <param name="elementSelector">
    /// The CSS selector that uniquely identifies this element in the rendered HTML
    /// (e.g., "#myButton" or "[data-xt-id=\"xt-button-1\"]").
    /// </param>
    /// <param name="output">The output container to which CSS rules and data attributes are written.</param>
    void Process(
        IntermediateRepresentationElement element,
        string elementSelector,
        TriggerOutput output);
}
