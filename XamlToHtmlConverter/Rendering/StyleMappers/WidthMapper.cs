using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class WidthMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
        => propertyName == "Width";

    public void Apply(
        IntermediateRepresentationElement element,
        string value,
        LayoutContext context,
        StringBuilder styleBuilder)
    {
        if (int.TryParse(value, out var w))
            styleBuilder.Append($"width:{w}px;");
    }
}