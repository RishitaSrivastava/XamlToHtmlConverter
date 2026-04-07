// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements;

/// <summary>
/// Coordinates a collection of property element handlers to process
/// complex XAML property elements (e.g., Grid.RowDefinitions, Control.Template).
/// Routes XML elements to the appropriate handler based on element name matching.
/// Uses caching to convert O(n) handler lookups into O(1) for repeated element names.
/// </summary>
public class PropertyElementHandlerEngine
{
    #region Private Data

    /// <summary>
    /// Holds the collection of registered property element handlers.
    /// </summary>
    private readonly List<IPropertyElementHandler> handlers;

    /// <summary>
    /// Cache mapping element names to their matching handler (or null if no match found).
    /// Significantly accelerates repeated lookups for common element names.
    /// Lazy-initialized on first access to minimize startup overhead.
    /// </summary>
    private Dictionary<string, IPropertyElementHandler?>? handlerCache;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="PropertyElementHandlerEngine"/>
    /// with the specified collection of handlers.
    /// </summary>
    /// <param name="handlers">The collection of property element handlers to register.</param>
    public PropertyElementHandlerEngine(IEnumerable<IPropertyElementHandler> handlers)
    {
        this.handlers = handlers.ToList();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Attempts to find and invoke a handler for the specified XML element.
    /// If a matching handler is found, it processes the element and returns true.
    /// Results are cached to optimize repeated element name lookups.
    /// </summary>
    /// <param name="element">The XML element to handle.</param>
    /// <param name="ir">The target IR element to populate.</param>
    /// <param name="convert">A callback function to recursively convert child XML elements to IR elements.</param>
    /// <returns>
    /// <c>true</c> if a handler was found and successfully processed the element;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool TryHandle(
        XElement element,
        IntermediateRepresentationElement ir,
        Func<XElement, IntermediateRepresentationElement> convert)
    {
        var name = element.Name.LocalName;

        // Initialize cache on first access (lazy initialization)
        handlerCache ??= new Dictionary<string, IPropertyElementHandler?>(StringComparer.Ordinal);

        // Check cache first for O(1) lookup on repeated element names
        if (handlerCache.TryGetValue(name, out var cachedHandler))
        {
            if (cachedHandler == null)
                return false;

            cachedHandler.Handle(element, ir, convert);
            return true;
        }

        // Cache miss: find matching handler and cache result
        IPropertyElementHandler? handler = null;
        foreach (var h in handlers)
        {
            if (h.CanHandle(name))
            {
                handler = h;
                break;
            }
        }

        // Cache the result (including null for no match) to avoid re-scanning
        handlerCache[name] = handler;

        if (handler == null)
            return false;

        handler.Handle(element, ir, convert);
        return true;
    }

    #endregion
}