// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.Rendering.ControlRenderers;
using XamlToHtmlConverter.Rendering.Controls;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Provides a centralized factory for creating a fully configured
    /// <see cref="HtmlRenderer"/> instance with all default dependencies wired.
    /// </summary>
    public static class HtmlRendererFactory
    {
        #region Public Methods

        /// <summary>
        /// Creates and returns an <see cref="HtmlRenderer"/> configured with the default
        /// tag mapper, all standard layout renderers, the default style builder,
        /// and the default event extractor.
        /// </summary>
        /// <returns>A fully initialized <see cref="HtmlRenderer"/> instance.</returns>
        public static HtmlRenderer Create()
        {
            var layouts = new List<ILayoutRenderer>
            {
                new GridLayoutRenderer(),
                new StackPanelLayoutRenderer(),
                new DockPanelLayoutRenderer(),
                new WrapPanelLayoutRenderer(),
                
            };

            var controlRegistry = new ControlRendererRegistry(new IControlRenderer[]
            {
                new TextBoxRenderer(),
                new CheckBoxRenderer(),
                new ListBoxRenderer(),
                new ItemsControlRenderer()
            });



            return new HtmlRenderer(
                new DefaultElementTagMapper(),
                layouts,
                new DefaultStyleBuilder(),
                new DefaultEventExtractor(),
                controlRegistry
            );
        }

        #endregion
    }
}
