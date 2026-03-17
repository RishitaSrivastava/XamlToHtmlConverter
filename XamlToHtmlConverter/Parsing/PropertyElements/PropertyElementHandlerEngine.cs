// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements;

/// <summary>
/// Coordinates a collection of property element handlers to process
/// complex XAML property elements (e.g., Grid.RowDefinitions, Control.Template).
/// Routes XML elements to the appropriate handler based on element name matching.
/// </summary>
public class PropertyElementHandlerEngine
{
    #region Private Data

    /// <summary>
    /// Holds the collection of registered property element handlers.
    /// </summary>
    private readonly List<IPropertyElementHandler> handlers;

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

        var handler = handlers.FirstOrDefault(h => h.CanHandle(name));

        if (handler == null)
            return false;

        handler.Handle(element, ir, convert);
        return true;
    }

    #endregion
}