using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class BorderMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "BorderThickness" ||
               propertyName == "BorderBrush" ||
               propertyName == "CornerRadius" ||
               propertyName == "Background";
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
            case "BorderThickness":
                if (int.TryParse(value, out var thickness))
                {
                    sb.Append($"border-width:{thickness}px;");
                    sb.Append("border-style:solid;");
                }
                break;

            case "BorderBrush":
                sb.Append($"border-color:{value};");
                break;

            case "CornerRadius":
                if (int.TryParse(value, out var radius))
                {
                    sb.Append($"border-radius:{radius}px;");
                }
                break;

            case "Background":
                sb.Append($"background:{value};");
                break;
        }
    }
}