// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Defines a contract for applying layout-specific CSS styling behavior
    /// to a supported IR element type.
    /// </summary>
    public interface ILayoutRenderer
    {
        /// <summary>
        /// Determines whether this renderer can handle the specified IR element.
        /// </summary>
        /// <param name="element">The IR element to evaluate.</param>
        /// <returns><c>true</c> if this renderer supports the element type; otherwise, <c>false</c>.</returns>
        bool CanHandle(IntermediateRepresentationElement element);

        /// <summary>
        /// Appends layout-related CSS styles for the specified IR element
        /// to the provided string builder.
        /// </summary>
        /// <param name="element">The IR element whose layout is being rendered.</param>
        /// <param name="styleBuilder">The string builder to append CSS styles to.</param>
        void ApplyLayout(IntermediateRepresentationElement element, StringBuilder styleBuilder);
    }
}
