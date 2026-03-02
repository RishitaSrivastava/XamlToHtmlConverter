using System.Collections.Generic;

namespace XamlToWebViewApp.Core.IR
{
    /// <summary>
    /// Represents an Intermediate Representation (IR) element
    /// created from a XAML node during parsing.
    ///
    /// The IR acts as a neutral abstraction layer between:
    /// 1. XAML Parsing (input structure)
    /// 2. HTML Rendering (output generation)
    ///
    /// Each IR element stores:
    /// - Element type (Button, StackPanel, TextBlock, etc.)
    /// - Element properties/attributes
    /// - Child elements (hierarchical UI structure)
    /// - Inner textual content (if present)
    /// </summary>
    public class IrElement
    {
        /// <summary>
        /// Gets or sets the XAML control type.
        /// Example: Button, TextBlock, StackPanel.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Stores element attributes such as Text, Content,
        /// Width, Height, etc.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
            = new Dictionary<string, string>();

        /// <summary>
        /// Child IR elements representing nested UI hierarchy.
        /// </summary>
        public List<IrElement> Children { get; set; }
            = new List<IrElement>();

        /// <summary>
        /// Direct textual content inside the element.
        /// Example:
        /// &lt;TextBlock&gt;Hello World&lt;/TextBlock&gt;
        /// </summary>
        public string? InnerText { get; set; }
    }
}