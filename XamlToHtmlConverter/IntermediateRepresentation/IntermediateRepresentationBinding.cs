namespace XamlToHtmlConverter.IntermediateRepresentation;

/// <summary>
/// Represents a parsed XAML binding expression.
/// </summary>
public class IntermediateRepresentationBinding
{
    public string? Path { get; set; }

    public string? Mode { get; set; }

    public string? ElementName { get; set; }

    public string? RelativeSource { get; set; }
}