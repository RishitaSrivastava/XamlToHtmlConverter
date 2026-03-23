using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Behavior;

public class BehaviorRegistry
{
    private readonly List<IBehaviorHandler> v_Handlers;

    public BehaviorRegistry(IEnumerable<IBehaviorHandler> handlers)
    {
        v_Handlers = handlers.ToList();
    }

    public Dictionary<string, string> Extract(
        IntermediateRepresentationElement element)
    {
        var result = new Dictionary<string, string>();

        foreach (var prop in element.Properties)
        {
            foreach (var handler in v_Handlers)
            {
                if (handler.CanHandle(prop.Key))
                {
                    handler.Apply(element, prop.Value, result);
                }
            }
        }

        return result;
    }
}