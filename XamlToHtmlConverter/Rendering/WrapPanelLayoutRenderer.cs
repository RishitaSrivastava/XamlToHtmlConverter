// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Layout renderer responsible for handling WrapPanel elements.
    /// Converts WrapPanel behavior into a flexbox CSS layout with wrapping enabled.
    /// Defaults to horizontal (row) direction when no Orientation is specified.
    /// </summary>
    public class WrapPanelLayoutRenderer : ILayoutRenderer
    {
        #region Public Methods

        /// <summary>
        /// Determines whether this renderer can handle the specified IR element.
        /// </summary>
        /// <param name="element">The IR element to evaluate.</param>
        /// <returns><c>true</c> if the element type is WrapPanel; otherwise, <c>false</c>.</returns>
        public bool CanHandle(IntermediateRepresentationElement element)
            => element.Type == "WrapPanel";

        /// <summary>
        /// Applies flexbox layout CSS rules for WrapPanel to the provided style builder.
        /// Enables flex-wrap and sets direction based on the Orientation property.
        /// </summary>
        /// <param name="element">The WrapPanel IR element to render layout for.</param>
        /// <param name="styleBuilder">The string builder to append CSS styles to.</param>
        public void ApplyLayout(IntermediateRepresentationElement element, StringBuilder styleBuilder)
        {
            styleBuilder.Append("display:flex;");
            styleBuilder.Append("flex-wrap:wrap;");

            var orientation = "Horizontal";
            if (element.Properties.TryGetValue("Orientation", out var o))
                orientation = o;

            if (string.Equals(orientation, "Vertical", StringComparison.OrdinalIgnoreCase))
                styleBuilder.Append("flex-direction:column;");
            else
                styleBuilder.Append("flex-direction:row;");
        }

        #endregion
    }
}