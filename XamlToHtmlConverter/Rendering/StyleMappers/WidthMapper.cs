using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class WidthMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "Width";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string propertyName,
        LayoutContext context,
        StringBuilder sb)
    {
        var value = element.Properties[propertyName];

        if (int.TryParse(value, out var width))
        {
            sb.Append($"width:{width}px;");
        }
    }
}