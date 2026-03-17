using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Behavior;

public static class BehaviorExtractor
{
    public static Dictionary<string, string> Extract(
    IntermediateRepresentationElement element)
    {
        var behaviors = new Dictionary<string, string>();

        foreach (var prop in element.Properties)
        {
            switch (prop.Key)
            {
                // EVENTS
                case "Click":
                    behaviors["data-event-click"] = prop.Value;
                    break;

                case "Loaded":
                    behaviors["data-event-loaded"] = prop.Value;
                    break;

                case "MouseEnter":
                    behaviors["data-event-mouseenter"] = prop.Value;
                    break;

                case "MouseLeave":
                    behaviors["data-event-mouseleave"] = prop.Value;
                    break;

                // STATE BINDINGS
                case "IsEnabled":
                    behaviors["data-binding-enabled"] = ExtractBinding(prop.Value);
                    break;

                case "Visibility":
                    behaviors["data-binding-visibility"] = ExtractBinding(prop.Value);
                    break;

                case "IsChecked":
                    behaviors["data-binding-checked"] = ExtractBinding(prop.Value);
                    break;

                case "IsSelected":
                    behaviors["data-binding-selected"] = ExtractBinding(prop.Value);
                    break;
            }
        }

        return behaviors;
    }

    private static string ExtractBinding(string value)
    {
        if (!value.StartsWith("{Binding"))
            return value;

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