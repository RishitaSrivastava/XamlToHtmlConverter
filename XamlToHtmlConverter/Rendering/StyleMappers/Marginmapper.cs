using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class MarginMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "Margin";
    }

    /// <summary>
    /// Parses thickness values in both space-separated (XAML: "0 5")
    /// and comma-separated (internal: "0,5") formats.
    /// </summary>
    private static string[] ParseThickness(string value)
    {
        // Try comma-separated first
        if (value.Contains(','))
            return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        
        // Fall back to space-separated (XAML format)
        return value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string propertyName,
        LayoutContext context,
        StringBuilder sb)
    {
        var value = element.Properties[propertyName];

        if (string.IsNullOrWhiteSpace(value))
            return;

        var parts = ParseThickness(value);

        // Margin="10"
        if (parts.Length == 1 && int.TryParse(parts[0], out var all))
        {
            sb.Append($"margin:{all}px;");
            return;
        }

        // Margin="10,5"  - WPF format: "horizontalMargin,verticalMargin"
        // Converts to CSS: margin: top right bottom left = vertical horizontal vertical horizontal
        if (parts.Length == 2)
        {
            if (int.TryParse(parts[0], out var horizontal) &&
                int.TryParse(parts[1], out var vertical))
            {
                // CSS margin format: top right bottom left
                sb.Append($"margin:{vertical}px {horizontal}px;");
            }

            return;
        }

        // Margin="5,10,5,10" or interpreted as left,top,right,bottom
        if (parts.Length == 4)
        {
            if (int.TryParse(parts[0], out var left) &&
                int.TryParse(parts[1], out var top) &&
                int.TryParse(parts[2], out var right) &&
                int.TryParse(parts[3], out var bottom))
            {
                sb.Append($"margin:{top}px {right}px {bottom}px {left}px;");
            }
        }
    }
}