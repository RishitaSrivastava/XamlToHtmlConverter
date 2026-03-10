using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class HeightMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
        => propertyName == "Height";

    public void Apply(
        IntermediateRepresentationElement element,
        string value,
        LayoutContext context,
        StringBuilder styleBuilder)
    {
        if (int.TryParse(value, out var h))
            styleBuilder.Append($"height:{h}px;");
    }
}