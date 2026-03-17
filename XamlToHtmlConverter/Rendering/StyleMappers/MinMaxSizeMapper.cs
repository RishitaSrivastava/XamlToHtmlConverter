using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class MinMaxSizeMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "MinWidth"
            || propertyName == "MaxWidth"
            || propertyName == "MinHeight"
            || propertyName == "MaxHeight";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string propertyName,
        LayoutContext context,
        StringBuilder sb)
    {
        var value = element.Properties[propertyName];

        if (!int.TryParse(value, out var size))
            return;

        switch (propertyName)
        {
            case "MinWidth":
                sb.Append($"min-width:{size}px;");
                break;

            case "MaxWidth":
                sb.Append($"max-width:{size}px;");
                break;

            case "MinHeight":
                sb.Append($"min-height:{size}px;");
                break;

            case "MaxHeight":
                sb.Append($"max-height:{size}px;");
                break;
        }
    }
}