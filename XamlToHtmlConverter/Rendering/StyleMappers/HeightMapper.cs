using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class HeightMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "Height";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string propertyName,
        LayoutContext context,
        StringBuilder sb)
    {
        var value = element.Properties[propertyName];

        if (int.TryParse(value, out var height))
        {
            sb.Append($"height:{height}px;");
        }
    }
}