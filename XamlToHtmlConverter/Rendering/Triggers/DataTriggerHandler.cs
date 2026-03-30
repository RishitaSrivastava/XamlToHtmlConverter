// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Handles data-bound triggers (<see cref="IntermediateRepresentationDataTrigger"/>).
///
/// DataTriggers depend on runtime binding evaluation and have no CSS equivalent.
/// This handler deliberately produces no output — neither data attributes nor
/// CSS rules. The element renders with its base styles (graceful degradation).
/// </summary>
public sealed class DataTriggerHandler : ITriggerHandler
{
    /// <inheritdoc />
    public void Process(
        IntermediateRepresentationElement element,
        string elementSelector,
        TriggerOutput output)
    {
        // DataTriggers require runtime binding evaluation.
        // No CSS equivalent exists; no data-datatrigger-* attributes are emitted.
        // Graceful degradation: element renders with its base/static styles.
    }
}
