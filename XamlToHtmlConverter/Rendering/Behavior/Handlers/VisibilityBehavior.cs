using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Behavior.Handlers;

public class VisibilityBehavior : IBehaviorHandler
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "Visibility";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string value,
        Dictionary<string, string> output)
    {
        output["data-binding-visibility"] = value;
    }
}