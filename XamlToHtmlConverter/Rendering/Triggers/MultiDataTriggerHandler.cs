// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Triggers;

/// <summary>
/// Handles multi-condition data-bound triggers (<see cref="IntermediateRepresentationMultiDataTrigger"/>).
///
/// MultiDataTriggers require runtime binding evaluation and have no CSS equivalent.
/// This handler deliberately produces no output (graceful degradation).
/// </summary>
public sealed class MultiDataTriggerHandler : ITriggerHandler
{
    /// <inheritdoc />
    public void Process(
        IntermediateRepresentationElement element,
        string elementSelector,
        TriggerOutput output)
    {
        // MultiDataTriggers require runtime binding evaluation.
        // No CSS equivalent exists; no data-multidatatrigger-* attributes are emitted.
    }
}
