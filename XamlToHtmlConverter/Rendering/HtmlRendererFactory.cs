// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.Rendering.Behavior;
using XamlToHtmlConverter.Rendering.Behavior.Handlers;
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
        #region Private Data

        /// <summary>
        /// Cached layout renderers collection. Prevents allocation on every Create() call.
        /// </summary>
        private static readonly ILayoutRenderer[] s_DefaultLayouts = new ILayoutRenderer[]
        {
            new GridLayoutRenderer(),
            new StackPanelLayoutRenderer(),
            new DockPanelLayoutRenderer(),
            new WrapPanelLayoutRenderer(),
            new ScrollViewerLayoutRenderer()
        };

        /// <summary>
        /// Cached control renderers collection.
        /// </summary>
        private static readonly IControlRenderer[] s_DefaultControlRenderers = new IControlRenderer[]
        {
            new TextBoxRenderer(),
            new CheckBoxRenderer(),
            new ListBoxRenderer(),
            new ItemsControlRenderer(),
        };

        /// <summary>
        /// Cached behavior handlers collection.
        /// </summary>
        private static readonly IBehaviorHandler[] s_DefaultBehaviorHandlers = new IBehaviorHandler[]
        {
            new ClickBehavior(),
            new EnabledBehavior(),
            new VisibilityBehavior(),
            new CommandBehavior(),
            new CheckedBehavior(),
            new SelectedBehavior(),
            new TriggerBehavior()
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates and returns an <see cref="HtmlRenderer"/> configured with the default
        /// tag mapper, all standard layout renderers, the default style builder,
        /// and the default event extractor.
        /// </summary>
        /// <returns>A fully initialized <see cref="HtmlRenderer"/> instance.</returns>
        public static HtmlRenderer Create()
        {
            var controlRegistry = new ControlRendererRegistry(s_DefaultControlRenderers);
            var behaviorRegistry = new BehaviorRegistry(s_DefaultBehaviorHandlers);

            return new HtmlRenderer(
                new DefaultElementTagMapper(),
                s_DefaultLayouts,
                new DefaultStyleBuilder(),
                new DefaultEventExtractor(),
                controlRegistry,
                behaviorRegistry
            );
        }

        #endregion
    }
}
