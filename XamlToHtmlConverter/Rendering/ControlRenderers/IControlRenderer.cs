using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.ControlRenderers;

public interface IControlRenderer
{
    bool CanHandle(IntermediateRepresentationElement element);

    void RenderAttributes(
        IntermediateRepresentationElement element,
        AttributeBuffer attributes);

    void RenderContent(
        IntermediateRepresentationElement element,
        StringBuilder sb,
        int indent,
        Action<IntermediateRepresentationElement, StringBuilder, int> renderChild);
}