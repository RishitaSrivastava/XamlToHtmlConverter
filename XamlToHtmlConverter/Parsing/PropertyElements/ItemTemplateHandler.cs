using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements;

public class ItemTemplateHandler : IPropertyElementHandler
{
    public bool CanHandle(string elementName)
    {
        return elementName.EndsWith(".ItemTemplate");
    }

    public void Handle(
        XElement node,
        IntermediateRepresentationElement ir,
        Func<XElement, IntermediateRepresentationElement> convert)
    {
        var dataTemplate = node.Elements().FirstOrDefault();

        if (dataTemplate == null)
            return;

        var templateRoot = dataTemplate.Elements().FirstOrDefault();

        if (templateRoot == null)
            return;

        var templateIr = convert(templateRoot);

        templateIr.Parent = ir;

        ir.ItemTemplate = templateIr;
    }
}