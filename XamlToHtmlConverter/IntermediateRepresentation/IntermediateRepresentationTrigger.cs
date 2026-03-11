namespace XamlToHtmlConverter.IntermediateRepresentation;

public class IntermediateRepresentationTrigger
{
    public string Property { get; set; } = "";
    public string Value { get; set; } = "";

    public Dictionary<string, string> Setters { get; } = new();
}