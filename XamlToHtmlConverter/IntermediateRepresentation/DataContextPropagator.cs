// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Propagates DataContext values down the IR element tree,
/// ensuring child elements inherit their parent's DataContext
/// when they don't define their own.
/// </summary>
public static class DataContextPropagator
{
    #region Public Methods

    /// <summary>
    /// Recursively propagates DataContext from parent to child elements.
    /// If an element already has a DataContext, it uses that value;
    /// otherwise, it inherits the parent's DataContext.
    /// </summary>
    /// <param name="element">The IR element to process.</param>
    /// <param name="parentContext">The DataContext value from the parent element, or null if none exists.</param>
    public static void Propagate(IntermediateRepresentationElement element, string? parentContext)
    {
        if (element.DataContext == null)
        {
            element.DataContext = parentContext;
        }

        foreach (var child in element.Children)
        {
            Propagate(child, element.DataContext);
        }
    }

    #endregion
}