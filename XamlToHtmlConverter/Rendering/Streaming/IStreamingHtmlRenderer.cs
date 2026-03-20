// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Interface for streaming HTML rendering implementations.
/// Enables rendering directly to a TextWriter without building the entire HTML in memory.
/// 
/// Benefits:
/// - Constant memory usage (no memory spikes for large documents)
/// - Lower peak memory requirements
/// - Faster time-to-first-byte
/// - Compatible with streaming outputs (file, network, pipe)
/// </summary>
public interface IStreamingHtmlRenderer
{
    /// <summary>
    /// Renders an IR element tree directly to a TextWriter.
    /// Output is written incrementally as rendering progresses.
    /// No intermediate string buffering for the entire document.
    /// </summary>
    /// <param name="element">The root IR element to render.</param>
    /// <param name="writer">The TextWriter to write HTML output to.</param>
    /// <remarks>
    /// Implementation should:
    /// - Write opening tags immediately
    /// - Flush periodically for responsiveness
    /// - Handle indentation and formatting efficiently
    /// - NOT buffer the entire document in memory
    /// </remarks>
    void RenderToStream(IntermediateRepresentationElement element, TextWriter writer);

    /// <summary>
    /// Renders an entire IR document to a file using streaming.
    /// Preferred method for large documents.
    /// </summary>
    /// <param name="element">The root IR element to render.</param>
    /// <param name="filePath">Path to write the HTML file to.</param>
    /// <remarks>
    /// Uses FileStream with appropriate buffering for optimal performance.
    /// Creates or overwrites the file.
    /// </remarks>
    void RenderToFile(IntermediateRepresentationElement element, string filePath);

    /// <summary>
    /// Renders to an in-memory StringBuilder but using streaming technique.
    /// Useful for testing or when memory buffering is acceptable.
    /// </summary>
    string RenderToString(IntermediateRepresentationElement element);
}
