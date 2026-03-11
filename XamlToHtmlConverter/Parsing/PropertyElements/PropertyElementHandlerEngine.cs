using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements;

public class PropertyElementHandlerEngine
{
    private readonly List<IPropertyElementHandler> handlers;

    public PropertyElementHandlerEngine(IEnumerable<IPropertyElementHandler> handlers)
    {
        this.handlers = handlers.ToList();
    }

    public bool TryHandle(
        XElement element,
        IntermediateRepresentationElement ir,
        Func<XElement, IntermediateRepresentationElement> convert)
    {
        var name = element.Name.LocalName;

        var handler = handlers.FirstOrDefault(h => h.CanHandle(name));

        if (handler == null)
            return false;

        handler.Handle(element, ir, convert);
        return true;
    }
}