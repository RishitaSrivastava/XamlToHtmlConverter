using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class MarginMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "Margin";
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

        var parts = value.Split(',');

        // Margin="10"
        if (parts.Length == 1 && int.TryParse(parts[0], out var all))
        {
            sb.Append($"margin:{all}px;");
            return;
        }

        // Margin="10,5"
        if (parts.Length == 2)
        {
            if (int.TryParse(parts[0], out var vertical) &&
                int.TryParse(parts[1], out var horizontal))
            {
                sb.Append($"margin:{vertical}px {horizontal}px;");
            }

            return;
        }

        // Margin="5,10,5,10"
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