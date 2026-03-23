using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Behavior.Handlers;

public class ClickBehavior : IBehaviorHandler
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "Click";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string value,
        Dictionary<string, string> output)
    {
        output["data-event-click"] = value;
    }
}