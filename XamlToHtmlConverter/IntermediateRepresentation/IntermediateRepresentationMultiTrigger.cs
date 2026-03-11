namespace XamlToHtmlConverter.IntermediateRepresentation;

public class IntermediateRepresentationMultiTrigger
{
    public List<(string Property, string Value)> Conditions { get; }
        = new();

    public Dictionary<string, string> Setters { get; }
        = new();
}