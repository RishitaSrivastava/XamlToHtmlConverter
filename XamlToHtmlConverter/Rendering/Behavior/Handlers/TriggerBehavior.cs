using XamlToHtmlConverter.IntermediateRepresentation;
using XamlToHtmlConverter.Rendering.Triggers;

namespace XamlToHtmlConverter.Rendering.Behavior.Handlers;

public class TriggerBehavior : IBehaviorHandler
{
    public bool CanHandle(string propertyName)
    {
        return false;
        // triggers are not property-based
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string value,
        Dictionary<string, string> output)
    {
        var triggers = TriggerEngine.Extract(element);

        foreach (var t in triggers)
            output[t.Key] = t.Value;
    }
}