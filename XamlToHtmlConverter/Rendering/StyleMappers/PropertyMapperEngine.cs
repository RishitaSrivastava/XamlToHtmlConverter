using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.StyleMappers;

public class PropertyMapperEngine
{
    private readonly List<IPropertyMapper> v_Mappers;

    public PropertyMapperEngine(IEnumerable<IPropertyMapper> mappers)
    {
        v_Mappers = mappers.ToList();
    }

    public void Apply(
        IntermediateRepresentationElement element,
        LayoutContext context,
        StringBuilder styleBuilder)
    {
        foreach (var prop in element.Properties)
        {
            Console.WriteLine($"Property found: {prop.Key} = {prop.Value}");
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