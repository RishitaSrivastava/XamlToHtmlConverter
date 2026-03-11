using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering.Triggers;

public static class TriggerEngine
{
    public static Dictionary<string, string> Extract(
        IntermediateRepresentationElement element)
    {
        var result = new Dictionary<string, string>();

        foreach (var trigger in element.Triggers)
        {
            var key = $"data-trigger-{trigger.Property.ToLower()}";

            var style = string.Join(";",
                trigger.Setters.Select(s => $"{s.Key}:{s.Value}"));

            result[key] = $"{trigger.Value}:{style}";
        }
        Console.WriteLine($"Triggers found: {element.Triggers.Count}");
        return result;
    }

    public static Dictionary<string, string> ExtractMultiTriggers(
    IntermediateRepresentationElement element)
    {
        var result = new Dictionary<string, string>();

        foreach (var trigger in element.MultiTriggers)
        {
            var conditionString = string.Join("&",
                trigger.Conditions.Select(c =>
                    $"{c.Property}={c.Value}"));

            var setterString = string.Join(";",
                trigger.Setters.Select(s =>
                    $"{s.Key}:{s.Value}"));

            result[$"data-multitrigger"] =
                $"{conditionString}:{setterString}";
        }

        return result;
    }

}