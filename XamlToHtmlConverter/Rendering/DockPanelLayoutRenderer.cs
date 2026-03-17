// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Layout renderer responsible for handling DockPanel elements.
    /// Converts DockPanel behavior into a flexbox-based CSS layout.
    /// Uses column direction when any child is docked to Top or Bottom;
    /// otherwise defaults to horizontal (row) layout.
    /// </summary>
    public class DockPanelLayoutRenderer : ILayoutRenderer
    {
        public int Priority => 70;
        #region Public Methods

        /// <summary>
        /// Determines whether this renderer can handle the specified IR element.
        /// </summary>
        /// <param name="element">The IR element to evaluate.</param>
        /// <returns><c>true</c> if the element type is DockPanel; otherwise, <c>false</c>.</returns>
        public bool CanHandle(IntermediateRepresentationElement element)
            => element.Type == "DockPanel";

        /// <summary>
        /// Applies flexbox layout CSS rules for DockPanel to the provided style builder.
        /// Uses column direction if any child is docked to Top or Bottom;
        /// otherwise defaults to row direction.
        /// </summary>
        /// <param name="element">The DockPanel IR element to render layout for.</param>
        /// <param name="styleBuilder">The string builder to append CSS styles to.</param>
        public void ApplyLayout(
    IntermediateRepresentationElement element,
    StringBuilder styleBuilder)
        {
            styleBuilder.Append("display:flex;");

            bool hasTopOrBottom = false;

            foreach (var child in element.Children)
            {
                if (!child.AttachedProperties.TryGetValue("DockPanel.Dock", out var dock))
                    continue;

                if (dock == "Top" || dock == "Bottom")
                {
                    hasTopOrBottom = true;
                    break;
                }
            }

            if (hasTopOrBottom)
                styleBuilder.Append("flex-direction:column;");
            else
                styleBuilder.Append("flex-direction:row;");
        }
        #endregion
    }
}