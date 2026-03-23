// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.Rendering;

/// <summary>
/// Defines a contract for managing CSS styles and generating style blocks.
/// Enables dependency inversion and testability of HtmlRenderer.
/// 
/// Replaces the hardcoded StyleRegistry field initialization in HtmlRenderer
/// with injected dependency, satisfying Dependency Inversion Principle.
/// </summary>
public interface IStyleRegistry
{
    /// <summary>
    /// Registers a CSS style string and returns a corresponding CSS class name.
    /// Reuses existing class names when an identical style string is already registered.
    /// </summary>
    /// <param name="cssStyle">The inline CSS style string to register.</param>
    /// <returns>The generated or existing CSS class name for the provided style string.</returns>
    string Register(string? cssStyle);

    /// <summary>
    /// Generates a complete HTML style block containing all registered CSS class definitions.
    /// Called once at the beginning of HTML rendering.
    /// </summary>
    /// <returns>A string containing a &lt;style&gt; block with all registered CSS class rules.</returns>
    string GenerateStyleBlock();
}
