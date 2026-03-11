using System.Xml.Linq;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Parsing.PropertyElements;

public interface IPropertyElementHandler
{
    bool CanHandle(string elementName);

    void Handle(
        XElement node,
        IntermediateRepresentationElement ir,
        Func<XElement, IntermediateRepresentationElement> convert);
}