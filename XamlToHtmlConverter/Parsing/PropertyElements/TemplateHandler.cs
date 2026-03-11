using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements;

public class TemplateHandler : IPropertyElementHandler
{
    public bool CanHandle(string elementName)
    {
        return elementName.EndsWith(".Template");
    }

    public void Handle(
        XElement node,
        IntermediateRepresentationElement ir,
        Func<XElement, IntermediateRepresentationElement> convert)
    {
        var templateElement = node.Elements().FirstOrDefault();

        if (templateElement == null)
            return;

        var templateIr = convert(templateElement);

        templateIr.Parent = ir;

        ir.Template = templateIr;
    }
}