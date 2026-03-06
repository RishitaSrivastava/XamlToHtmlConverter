// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Represents contextual information about the parent layout container
    /// used during CSS style generation for child elements.
    /// </summary>
    public class LayoutContext
    {
        #region Public Properties

        /// <summary>
        /// Gets the layout type of the parent container element
        /// (e.g., Grid, StackPanel, DockPanel).
        /// </summary>
        public string? ParentLayoutType { get; }

        /// <summary>
        /// Gets the orientation of the parent container element
        /// (e.g., Horizontal, Vertical) when applicable.
        /// </summary>
        public string? ParentOrientation { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="LayoutContext"/> with the specified parent layout type
        /// and an optional orientation.
        /// </summary>
        /// <param name="parentLayoutType">The layout type of the parent container (e.g., Grid, StackPanel).</param>
        /// <param name="parentOrientation">The orientation of the parent container (e.g., Horizontal, Vertical).</param>
        public LayoutContext(string? parentLayoutType, string? parentOrientation = null)
        {
            ParentLayoutType = parentLayoutType;
            ParentOrientation = parentOrientation;
        }

        #endregion
    }
}
