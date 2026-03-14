// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Layout renderer responsible for handling StackPanel elements.
    /// Converts StackPanel behavior into corresponding flexbox CSS layout styles.
    /// Defaults to vertical (column) direction when no Orientation is specified.
    /// </summary>
    public class StackPanelLayoutRenderer : ILayoutRenderer
    {
        public int Priority => 80;
        #region Public Methods

        /// <summary>
        /// Determines whether this renderer can handle the specified IR element.
        /// </summary>
        /// <param name="element">The IR element to evaluate.</param>
        /// <returns><c>true</c> if the element type is StackPanel; otherwise, <c>false</c>.</returns>
        public bool CanHandle(IntermediateRepresentationElement element)
            => element.Type == "StackPanel";

        /// <summary>
        /// Applies flexbox layout CSS styles based on the StackPanel's Orientation property.
        /// Emits column direction for Vertical (default) or row direction for Horizontal.
        /// </summary>
        /// <param name="element">The StackPanel IR element to render layout for.</param>
        /// <param name="styleBuilder">The string builder to append CSS styles to.</param>
        public void ApplyLayout(IntermediateRepresentationElement element, StringBuilder styleBuilder)
        {
            styleBuilder.Append("display:flex;");

            var direction = "column";
            if (element.Properties.TryGetValue("Orientation", out var orientation))
            {
                if (orientation.Equals("Horizontal", StringComparison.OrdinalIgnoreCase))
                    direction = "row";
            }
            styleBuilder.Append($"flex-direction:{direction};");
            
        }

        #endregion
    }
}
