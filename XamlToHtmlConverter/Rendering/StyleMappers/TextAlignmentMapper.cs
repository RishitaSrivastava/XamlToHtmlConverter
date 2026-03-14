using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class TextAlignmentMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "TextAlignment";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string propertyName,
        LayoutContext context,
        StringBuilder sb)
    {
        var value = element.Properties[propertyName];

        sb.Append($"text-align:{value.ToLower()};");
    }
}