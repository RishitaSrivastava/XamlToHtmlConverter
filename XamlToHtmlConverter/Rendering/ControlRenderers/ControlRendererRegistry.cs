// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

/// <summary>
/// Manages a collection of control renderers and provides
/// resolution logic to find the appropriate renderer for a given IR element.
/// </summary>
public class ControlRendererRegistry
{
    #region Private Data

    /// <summary>
    /// Holds the collection of registered control renderers.
    /// </summary>
    private readonly List<IControlRenderer> v_Renderers;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="ControlRendererRegistry"/>
    /// with the specified collection of control renderers.
    /// </summary>
    /// <param name="renderers">The collection of control renderers to register.</param>
    public ControlRendererRegistry(IEnumerable<IControlRenderer> renderers)
    {
        v_Renderers = renderers.ToList();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Finds and returns the first control renderer that can handle the specified IR element.
    /// Logs the resolution process to the console for debugging purposes.
    /// </summary>
    /// <param name="element">The IR element to find a renderer for.</param>
    /// <returns>
    /// The first <see cref="IControlRenderer"/> that can handle the element,
    /// or <c>null</c> if no suitable renderer is found.
    /// </returns>
    public IControlRenderer? Resolve(IntermediateRepresentationElement element)
    {
        foreach (var r in v_Renderers)
        {
            if (r.CanHandle(element))
            {
                return r;
            }
        }

        return null;
    }

    #endregion
}