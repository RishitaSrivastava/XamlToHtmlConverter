using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class TypographyMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "FontSize" ||
               propertyName == "FontWeight" ||
               propertyName == "FontFamily" ||
               propertyName == "Foreground" ||
               propertyName == "FontStyle" ||
               propertyName == "TextDecorations" ||
               propertyName == "Opacity";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string propertyName,
        LayoutContext context,
        StringBuilder sb)
    {
        var value = element.Properties[propertyName];

        switch (propertyName)
        {
            case "FontSize":
                if (int.TryParse(value, out var size))
                    sb.Append($"font-size:{size}px;");
                break;

            case "FontWeight":
                sb.Append($"font-weight:{value.ToLower()};");
                break;

            case "FontFamily":
                sb.Append($"font-family:{value};");
                break;

            case "Foreground":
                sb.Append($"color:{value};");
                break;

            case "FontStyle":
                sb.Append($"font-style:{value.ToLower()};");
                break;

            case "TextDecorations":
                sb.Append($"text-decoration:{value.ToLower()};");
                break;

            case "Opacity":
                if (double.TryParse(value, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var opacity))
                    sb.Append($"opacity:{opacity};");
                break;
        }
    }
}