using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public interface IPropertyMapper
{
    bool CanHandle(string propertyName);

    void Apply(
        IntermediateRepresentationElement element,
        string propertyValue,
        LayoutContext context,
        StringBuilder styleBuilder);
}