using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Behavior;

public interface IBehaviorHandler
{
    bool CanHandle(string propertyName);

    void Apply(
        IntermediateRepresentationElement element,
        string propertyValue,
        Dictionary<string, string> output);
}