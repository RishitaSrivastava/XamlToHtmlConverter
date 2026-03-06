// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Defines a contract for extracting event-related metadata
    /// from IR elements during HTML rendering.
    /// </summary>
    public interface IEventExtractor
    {
        /// <summary>
        /// Extracts event attributes from the specified IR element
        /// and returns them as HTML-compatible key-value pairs.
        /// </summary>
        /// <param name="element">The IR element to scan for event properties.</param>
        /// <returns>
        /// A dictionary of HTML attribute names to event handler names
        /// (e.g., data-event-click to OnButtonClick).
        /// </returns>
        Dictionary<string, string> Extract(IntermediateRepresentationElement element);
    }
}
