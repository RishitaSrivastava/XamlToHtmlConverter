using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class AlignmentMapper : IPropertyMapper
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "HorizontalAlignment"
            || propertyName == "VerticalAlignment";
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

        var cssValue = ConvertAlignment(value);

        if (propertyName == "HorizontalAlignment")
        {
            if (context.ParentLayoutType == "Grid")
                sb.Append($"justify-self:{cssValue};");

            if (context.ParentLayoutType == "StackPanel" &&
                context.ParentOrientation == "Vertical")
                sb.Append($"align-self:{cssValue};");
        }

        if (propertyName == "VerticalAlignment")
        {
            if (context.ParentLayoutType == "Grid")
                sb.Append($"align-self:{cssValue};");

            if (context.ParentLayoutType == "StackPanel" &&
                context.ParentOrientation == "Horizontal")
                sb.Append($"align-self:{cssValue};");
        }
    }

    private string ConvertAlignment(string value)
    {
        return value switch
        {
            "Left" => "start",
            "Top" => "start",
            "Right" => "end",
            "Bottom" => "end",
            "Center" => "center",
            "Stretch" => "stretch",
            _ => "stretch"
        };
    }
}