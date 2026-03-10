namespace XamlToHtmlConverter.IntermediateRepresentation;

public static class DataContextPropagator
{
    public static void Propagate(IntermediateRepresentationElement element, string? parentContext)
    {
        if (element.DataContext == null)
        {
            element.DataContext = parentContext;
        }

        foreach (var child in element.Children)
        {
            Propagate(child, element.DataContext);
        }
    }
}