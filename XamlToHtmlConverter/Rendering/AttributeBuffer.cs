// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Manages a collection of HTML element attributes and provides
    /// efficient serialization into a string builder.
    /// Prevents duplicate attribute names by using a dictionary structure.
    /// </summary>
    public class AttributeBuffer
    {
        #region Private Data

        /// <summary>
        /// Holds the mapping of attribute names to their values.
        /// </summary>
        private readonly Dictionary<string, string> v_Attributes = new();

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds or updates an HTML attribute in the buffer.
        /// Ignores null or whitespace names and values.
        /// </summary>
        /// <param name="name">The attribute name (e.g., "id", "class").</param>
        /// <param name="value">The attribute value.</param>
        public void Add(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                return;

            v_Attributes[name] = value;
        }

        /// <summary>
        /// Writes all buffered attributes to the provided string builder
        /// in HTML attribute format (space-separated name="value" pairs).
        /// </summary>
        /// <param name="sb">The string builder to append attributes to.</param>
        public void WriteTo(StringBuilder sb)
        {
            foreach (var attr in v_Attributes)
            {
                sb.Append(' ');
                sb.Append(attr.Key);
                sb.Append("=\"");
                sb.Append(attr.Value);
                sb.Append('"');
            }
        }

        #endregion
    }
}
