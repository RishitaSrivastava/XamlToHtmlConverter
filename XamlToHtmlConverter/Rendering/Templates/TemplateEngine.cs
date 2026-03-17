// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Templates;

/// <summary>
/// Processes and expands XAML control templates and item templates
/// within the IR element tree. Replaces template nodes with their
/// visual content, effectively "applying" the template definitions.
/// </summary>
public class TemplateEngine
{
    #region Public Methods

    /// <summary>
    /// Recursively processes the IR tree to expand control templates and item templates.
    /// Template nodes are replaced with their visual children, which are then added
    /// directly to the parent element.
    /// </summary>
    /// <param name="element">The root IR element to process.</param>
    public void ExpandTemplates(IntermediateRepresentationElement element)
    {
        for (int i = 0; i < element.Children.Count; i++)
        {
            var child = element.Children[i];

            // Handle ControlTemplate (e.g., Button.Template)
            if (child.Type.EndsWith(".Template"))
            {
                ExpandControlTemplate(element, child);
                element.Children.RemoveAt(i);
                i--;
                continue;
            }

            // Handle ItemTemplate (e.g., ItemsControl.ItemTemplate)
            if (child.Type == "ItemsControl.ItemTemplate")
            {
                ExpandItemTemplate(element, child);
                element.Children.RemoveAt(i);
                i--;
                continue;
            }

            // Recursively process child elements
            ExpandTemplates(child);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Expands a control template by extracting its visual content
    /// and adding it to the parent element's children.
    /// </summary>
    /// <param name="parent">The parent element that owns the template.</param>
    /// <param name="templateNode">The template node to expand.</param>
    private void ExpandControlTemplate(
        IntermediateRepresentationElement parent,
        IntermediateRepresentationElement templateNode)
    {
        foreach (var templateChild in templateNode.Children)
        {
            if (templateChild.Type == "ControlTemplate")
            {
                foreach (var visual in templateChild.Children)
                {
                    visual.Parent = parent;
                    parent.Children.Add(visual);
                }
            }
        }
    }

    /// <summary>
    /// Expands an item template by extracting its data template content
    /// and adding it to the parent element's children.
    /// </summary>
    /// <param name="parent">The parent element that owns the template.</param>
    /// <param name="templateNode">The template node to expand.</param>
    private void ExpandItemTemplate(
        IntermediateRepresentationElement parent,
        IntermediateRepresentationElement templateNode)
    {
        foreach (var templateChild in templateNode.Children)
        {
            if (templateChild.Type == "DataTemplate")
            {
                foreach (var visual in templateChild.Children)
                {
                    visual.Parent = parent;
                    parent.Children.Add(visual);
                }
            }
        }
    }

    #endregion
}