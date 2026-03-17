using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class PropertyMapperEngine
{
    private readonly IEnumerable<IPropertyMapper> v_Mappers;

    public PropertyMapperEngine(IEnumerable<IPropertyMapper> mappers)
    {
        v_Mappers = mappers;
    }

    public void Apply(
        IntermediateRepresentationElement element,
        LayoutContext context,
        StringBuilder styleBuilder)
    {
        foreach (var prop in element.Properties)
        {
            foreach (var mapper in v_Mappers)
            {
                if (mapper.CanHandle(prop.Key))
                {
                    mapper.Apply(element, prop.Key, context, styleBuilder);
                    break;
                }
            }
        }
    }
}