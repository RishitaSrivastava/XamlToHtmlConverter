using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Behavior.Handlers;

public class CheckedBehavior : IBehaviorHandler
{
    public bool CanHandle(string propertyName)
    {
        return propertyName == "IsChecked";
    }

    public void Apply(
        IntermediateRepresentationElement element,
        string value,
        Dictionary<string, string> output)
    {
        output["data-binding-checked"] = ExtractBinding(value);
    }

    private string ExtractBinding(string value)
    {
        if (!value.StartsWith("{Binding"))
            return value;

        var inner = value.Replace("{Binding", "")
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