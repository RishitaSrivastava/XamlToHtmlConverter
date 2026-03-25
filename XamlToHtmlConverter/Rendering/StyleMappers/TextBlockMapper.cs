using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

/// <summary>
/// Maps TextBlock-specific styling and applies default styling for visibility.
/// Ensures TextBlocks without explicit properties still render with proper spacing/visibility.
/// Implements IPropertyMapper but also provides element-type-specific styling.
/// </summary>
public class TextBlockMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        // This mapper doesn't handle specific properties;
        // it applies defaults based on element type.
        return false;
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string propertyName,
        LayoutContext context,
        StringBuilder sb)
    {
        // No-op: this mapper uses element type detection instead
    }

    /// <summary>
    /// Applies default TextBlock styling (margin, line-height) for visibility.
    /// Called directly from BuildStyle() to ensure all TextBlocks are visible.
    /// </summary>
    /// <param name="element">The IR element to style.</param>
    /// <param name="sb">The style builder to append CSS to.</param>
    public static void ApplyTextBlockStyle(IntermediateRepresentationElement element, StringBuilder sb)
    {
        // Only apply defaults if element is TextBlock or TextBlock-like
        if (element.Type != "TextBlock")
            return;

        // Apply default margin if not explicitly set
        if (!element.Properties.ContainsKey("Margin"))
        {
            sb.Append("margin:4px 0px;");
        }

        // Apply default line-height for readability
        sb.Append("line-height:1.4;");
    }
}
