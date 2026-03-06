// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Manages reusable CSS styles by mapping inline style strings
    /// to generated CSS class names and producing a consolidated HTML style block.
    /// Deduplicates styles so identical declarations share a single class.
    /// </summary>
    public class StyleRegistry
    {
        #region Private Data

        /// <summary>
        /// Holds the mapping of CSS style strings to generated class names.
        /// </summary>
        private readonly Dictionary<string, string> v_StyleToClass = new();

        /// <summary>
        /// Holds the reverse mapping of generated class names to their CSS style definitions.
        /// </summary>
        private readonly Dictionary<string, string> v_ClassToStyle = new();

        /// <summary>
        /// Holds the counter used to generate unique CSS class names.
        /// </summary>
        private int v_Counter = 1;

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a CSS style string and returns a corresponding CSS class name.
        /// Reuses the existing class name when an identical style string is already registered.
        /// </summary>
        /// <param name="style">The inline CSS style string to register.</param>
        /// <returns>The generated or existing CSS class name for the provided style string.</returns>
        public string Register(string? style)
        {
            if (string.IsNullOrWhiteSpace(style))
                return string.Empty;

            if (v_StyleToClass.TryGetValue(style, out var existing))
                return existing;

            var className = $"c{v_Counter++}";
            v_StyleToClass[style] = className;
            v_ClassToStyle[className] = style;

            return className;
        }

        /// <summary>
        /// Generates a complete HTML style block containing all registered CSS class definitions.
        /// </summary>
        /// <returns>A string containing a &lt;style&gt; block with all registered CSS class rules.</returns>
        public string GenerateStyleBlock()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<style>");

            foreach (var kvp in v_ClassToStyle)
                sb.AppendLine($".{kvp.Key} {{ {kvp.Value} }}");

            sb.AppendLine("</style>");
            return sb.ToString();
        }

        #endregion
    }
}
