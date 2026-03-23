using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Behavior.Handlers;

public class CommandBehavior : IBehaviorHandler
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "Command"
            || propertyName == "CommandParameter";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string value,
        Dictionary<string, string> output)
    {
        if (value.StartsWith("{Binding"))
        {
            value = ExtractBinding(value);
        }

        if (element.Properties.ContainsKey("Command"))
        {
            var commandValue = element.Properties["Command"];

            if (commandValue.StartsWith("{Binding"))
                commandValue = ExtractBinding(commandValue);

            output["data-command"] = commandValue;
        }

        if (element.Properties.ContainsKey("CommandParameter"))
        {
            var paramValue = element.Properties["CommandParameter"];

            if (paramValue.StartsWith("{Binding"))
                paramValue = ExtractBinding(paramValue);

            output["data-command-parameter"] = paramValue;
        }
    }

    private string ExtractBinding(string value)
    {
        var inner = value
            .Replace("{Binding", "")
            .Replace("}", "")
            .Trim();

        if (inner.Contains("Path="))
        {
            var parts = inner.Split(',');

            foreach (var p in parts)
            {
                var trimmed = p.Trim();

                if (trimmed.StartsWith("Path="))
                    return trimmed.Substring(5);
            }
        }

        return inner.Split(',')[0].Trim();
    }
}